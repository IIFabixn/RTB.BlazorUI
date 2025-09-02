using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace RTB.Blazor.Services.BusyIndicator
{
    /// <summary>
    /// Abstraction for tracking and reacting to "busy" states during asynchronous or long-running operations.
    /// Useful for driving UI concerns such as spinners, disabling actions, or detecting background work.
    /// </summary>
    /// <remarks>
    /// Thread-safe. Multiple independent scopes can be tracked per key.
    /// Keys are case-sensitive. Passing null or empty when supported indicates "any key".
    /// </remarks>
    public interface IBusyTracker
    {
        /// <summary>
        /// Raised whenever the busy state changes for any key (work started or finished).
        /// The string argument contains the affected key; it may be null when callers check "any" state.
        /// </summary>
        /// <remarks>
        /// In Blazor components, prefer invoking UI updates via InvokeAsync(StateHasChanged)
        /// from this callback to avoid threading issues.
        /// This event may fire frequently; keep handlers lightweight.
        /// </remarks>
        event Action<string?>? OnBusyChanged;

        /// <summary>
        /// Determines whether the specified key is currently busy.
        /// </summary>
        /// <param name="key">
        /// The key to check. When null or empty, this reports whether any key is busy (<see cref="IsAnyBusy"/>).
        /// </param>
        /// <returns>
        /// True if the specified key has an outstanding busy count greater than zero;
        /// or, when key is null/empty, true if any key is busy.
        /// </returns>
        bool IsBusy(string? key = null);

        /// <summary>
        /// Indicates whether any tracked key is currently busy.
        /// </summary>
        bool IsAnyBusy { get; }

        /// <summary>
        /// Begins tracking a busy scope for the specified key and returns a token that ends the scope on dispose.
        /// </summary>
        /// <param name="method">
        /// The key to track. Defaults to the caller's member name via <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        /// <param name="onDispose">
        /// Optional callback invoked after the busy scope is ended and the internal count is decremented/cleared.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> token. Dispose to end the busy scope. Intended to be used with a using statement.
        /// </returns>
        /// <example>
        /// using var _ = busyTracker.Track(); // key becomes the caller's method name
        /// await DoWorkAsync();
        /// // disposing '_' marks this scope as complete
        /// </example>
        IDisposable Track([CallerMemberName] string method = "", Action? onDispose = null);

        /// <summary>
        /// The set of keys that are currently busy (i.e., have a positive outstanding count).
        /// </summary>
        string[] Tracks { get; }

        /// <summary>
        /// Returns a task that completes when the specified key is no longer busy.
        /// </summary>
        /// <param name="key">The key to await completion for.</param>
        /// <returns>
        /// A task that completes when the key's busy count reaches zero.
        /// If the key is not currently busy, the task is already completed.
        /// </returns>
        /// <remarks>
        /// Only a single waiter is maintained per key; subsequent calls overwrite previous waiters.
        /// Continuations run asynchronously.
        /// </remarks>
        Task Await(string key);
    }

    /// <summary>
    /// Default implementation of <see cref="IBusyTracker"/> that tracks busy states by key
    /// using reference-counted scopes stored in concurrent dictionaries.
    /// </summary>
    /// <remarks>
    /// Thread-safe. Designed for UI scenarios (e.g., Blazor) to reflect background work state.
    /// Keys are case-sensitive. Disposing a tracking token decrements the count and may complete any awaiting task.
    /// </remarks>
    public class BusyTracker : IBusyTracker
    {
        // Tracks the active scope count per key.
        private readonly ConcurrentDictionary<string, int> _busyKeys = new();

        // Holds a single waiter per key that completes when the key becomes idle.
        private readonly ConcurrentDictionary<string, TaskCompletionSource> _waiters = new();

        /// <summary>
        /// The set of keys that are currently busy (i.e., have a positive outstanding count).
        /// </summary>
        public string[] Tracks => [.. _busyKeys.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key)];

        /// <summary>
        /// Raised when any busy state changes (work begins or ends).
        /// </summary>
        /// <remarks>
        /// The provided string argument is the key whose state changed.
        /// In Blazor components, marshal UI updates via InvokeAsync(StateHasChanged).
        /// </remarks>
        public event Action<string?>? OnBusyChanged;

        /// <inheritdoc />
        public bool IsBusy(string? key = "") =>
            string.IsNullOrEmpty(key) ? IsAnyBusy : _busyKeys.TryGetValue(key, out var count) && count > 0;

        /// <inheritdoc />
        public bool IsAnyBusy => _busyKeys.Values.Any(v => v > 0);

        /// <summary>
        /// Returns the current mapping of keys to their active scope counts.
        /// </summary>
        /// <remarks>
        /// This is a live, thread-safe view backed by a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// Contents can change while iterating; prefer enumerating promptly.
        /// </remarks>
        public IReadOnlyDictionary<string, int> CurrentState => _busyKeys;

        /// <inheritdoc />
        public IDisposable Track([CallerMemberName] string key = "", Action? onDispose = null)
        {
            Add(key);

            return new BusyToken(key, () =>
            {
                Remove(key);
                onDispose?.Invoke();
            });
        }

        /// <inheritdoc />
        public Task Await(string key)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            if (!IsBusy(key))
            {
                tcs.SetResult();
            }
            else
            {
                // Only one waiter is kept per key; the most recent caller wins.
                _waiters[key] = tcs;
            }

            return tcs.Task;
        }

        /// <summary>
        /// Increments the busy counter for the given key and notifies listeners.
        /// </summary>
        private void Add(string key)
        {
            if (_busyKeys.TryGetValue(key, out var count))
                _busyKeys[key] = ++count;
            else
                _busyKeys[key] = 1;

            OnBusyChanged?.Invoke(key);
        }

        /// <summary>
        /// Decrements the busy counter for the given key; removes it when the count reaches zero.
        /// Completes any pending waiter for the key when transitioning to idle.
        /// Notifies listeners of the state change.
        /// </summary>
        private void Remove(string key)
        {
            if (_busyKeys.TryGetValue(key, out int count))
            {
                count--;

                if (count > 0)
                {
                    _busyKeys[key] = count;
                }
                else
                {
                    if (_busyKeys.TryRemove(key, out _))
                    {
                        if (_waiters.TryRemove(key, out var waiter))
                        {
                            waiter.SetResult();
                        }
                    }
                }
            }

            OnBusyChanged?.Invoke(key);
        }

        /// <summary>
        /// Represents a tracked busy scope that automatically ends when disposed.
        /// Use with <c>using</c> to ensure the busy state is always released.
        /// </summary>
        /// <remarks>
        /// Instances are created by <see cref="BusyTracker.Track(string, Action?)"/>.
        /// The provided onDispose action (if any) is invoked after the busy state is released.
        /// </remarks>
        public readonly struct BusyToken(string key, Action? onDispose = null) : IDisposable
        {
            /// <summary>
            /// The key associated with this busy scope.
            /// </summary>
            public string Key { get; } = key;

            /// <summary>
            /// Ends the busy scope and invokes the registered dispose callback (if any).
            /// </summary>
            public void Dispose()
            {
                onDispose?.Invoke();
            }
        }
    }
}
