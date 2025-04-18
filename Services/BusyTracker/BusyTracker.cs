using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.BusyTracker
{
    /// <summary>
    /// Tracks busy states for asynchronous operations by named keys.
    /// Useful for showing spinners, disabling buttons, and detecting background tasks.
    /// Supports multiple concurrent scopes per key.
    /// </summary>
    public class BusyTracker(ILogger<BusyTracker> logger)
    {
        private readonly ConcurrentDictionary<string, int> _busyKeys = new();

        /// <summary>
        /// Raised when any busy state changes (e.g. start or end of tracked work).
        /// Components can subscribe to update UI accordingly.
        /// </summary>
        public event Action? OnBusyChanged;

        /// <summary>
        /// Checks if a specific key is currently busy.
        /// </summary>
        public bool IsBusy(string key) =>
            _busyKeys.TryGetValue(key, out var count) && count > 0;

        /// <summary>
        /// True if any tracked key is currently busy.
        /// </summary>
        public bool IsAnyBusy => _busyKeys.Values.Any(v => v > 0);

        /// <summary>
        /// Returns the current state of all tracked busy keys and their call counts.
        /// </summary>
        public IReadOnlyDictionary<string, int> CurrentState => _busyKeys;

        public BusyToken Track(string? prefix = null, [CallerMemberName] string? method = null)
        {
            var key = string.IsNullOrEmpty(prefix)
                ? $"{method}"
                : $"{prefix}.{method}";

            Add(key);

            return new BusyToken(this, key);
        }

        /// <summary>
        /// Increments the counter for the given busy key.
        /// </summary>
        private void Add(string key)
        {
            if (_busyKeys.TryGetValue(key, out var count))
                _busyKeys[key] = count + 1;
            else
                _busyKeys[key] = 1;

            logger.LogDebug($"BusyTracker: {key} is now busy ({_busyKeys[key]}).");

            OnBusyChanged?.Invoke();
        }

        /// <summary>
        /// Decrements the counter for the given busy key.
        /// Removes the key when the count reaches zero.
        /// </summary>
        private void Remove(string key)
        {
            if (_busyKeys.TryGetValue(key, out var count) && count > 0)
            {
                _busyKeys[key] = count - 1;

                // Clean up when no longer busy
                if (_busyKeys[key] == 0)
                    _busyKeys.TryRemove(key, out _);

                logger.LogDebug($"BusyTracker: {key} is no longer busy ({_busyKeys.Count} remaining).");

                OnBusyChanged?.Invoke();
            }
        }

        /// <summary>
        /// Represents a tracked busy scope that automatically ends when disposed.
        /// Use with <c>using</c> or <c>await using</c> to automatically release busy state.
        /// </summary>
        /// <remarks>
        /// Creates a new busy token. Should not be created manually — use <c>Track</c> or <c>TrackAsync</c>.
        /// </remarks>
        public readonly struct BusyToken(BusyTracker tracker, string key) : IDisposable
        {
            private readonly BusyTracker _tracker = tracker;
            private readonly string _key = key;

            /// <summary>
            /// Automatically ends the busy scope when disposed.
            /// </summary>
            public void Dispose()
            {
                _tracker.Remove(_key);
            }
        }
    }
}
