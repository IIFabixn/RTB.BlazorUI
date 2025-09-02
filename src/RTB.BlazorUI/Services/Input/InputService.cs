using Microsoft.JSInterop;
using System.Collections.Concurrent;

namespace RTB.Blazor.Services.Input;

/// <summary>
/// Provides a Blazor-friendly keyboard input subscription service that bridges DOM keyboard events
/// from JavaScript to .NET callbacks.
/// </summary>
/// <remarks>
/// - Call <see cref="InitializeAsync"/> once to attach JS listeners (typically at app startup or the first time a page needing input renders).
/// - Use <see cref="RegisterKeyHandler(string, Action{bool})"/> to subscribe to specific keys via <c>KeyboardEvent.key</c> values (e.g., "Shift", "Control", "a").
/// - Callbacks receive <c>true</c> on key down and <c>false</c> on key up.
/// - Use <see cref="UnregisterKeyHandler(string, Action{bool})"/> to remove subscriptions.
/// - Dispose the service (or its scope) to detach JS listeners.
/// Thread-safety: Handler lists are protected with locks on the per-key list; enumeration uses a snapshot to avoid concurrent modification.
/// </remarks>
public interface IInputService : IAsyncDisposable
{
    /// <summary>
    /// Subscribe a callback to a specific key as reported by <c>KeyboardEvent.key</c>.
    /// </summary>
    /// <param name="key">
    /// The key identifier string (e.g., "Shift", "Control", "Alt", "ArrowUp", "a", "A"). Use the exact value produced by <c>KeyboardEvent.key</c>.
    /// </param>
    /// <param name="callback">
    /// The callback invoked with <c>true</c> on key down and <c>false</c> on key up.
    /// </param>
    /// <remarks>
    /// Multiple handlers can be registered for the same key. Handlers are invoked in registration order.
    /// </remarks>
    void RegisterKeyHandler(string key, Action<bool> callback);

    /// <summary>
    /// Unsubscribe a previously registered callback for a specific key.
    /// </summary>
    /// <param name="key">The same key used during registration.</param>
    /// <param name="callback">The callback instance to remove.</param>
    void UnregisterKeyHandler(string key, Action<bool> callback);

    /// <summary>
    /// Initializes the JavaScript interop by attaching global keyboard listeners once.
    /// </summary>
    /// <returns>A task that completes when the JS listeners are attached.</returns>
    /// <remarks>
    /// This calls the JS function <c>inputService.register(dotNetObjRef)</c>. Ensure a corresponding JS implementation exists and is loaded.
    /// Safe to call multiple times; only the first call needs to wire up JS.
    /// </remarks>
    Task InitializeAsync();
}

/// <summary>
/// Default implementation of <see cref="IInputService"/> using <see cref="IJSRuntime"/> to receive keyboard events from the browser.
/// </summary>
/// <remarks>
/// Expects the following JS functions to exist:
/// - <c>inputService.register(dotNetObjRef)</c>: Attaches keydown/keyup listeners and forwards events via <see cref="OnKeyDown(string)"/> and <see cref="OnKeyUp(string)"/>.
/// - <c>inputService.unregister()</c>: Detaches listeners.
/// </remarks>
public class InputService(IJSRuntime jsRuntime) : IInputService
{
    private DotNetObjectReference<InputService>? _objRef;

    // Key -> list of callbacks. Lists are locked per-key; we snapshot before invoking to avoid modification during enumeration.
    private readonly ConcurrentDictionary<string, List<Action<bool>>> _handlers = new();

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _objRef = DotNetObjectReference.Create(this);
        await jsRuntime.InvokeVoidAsync("inputService.register", _objRef);
    }

    /// <inheritdoc />
    public void RegisterKeyHandler(string key, Action<bool> callback)
    {
        var list = _handlers.GetOrAdd(key, _ => []);
        lock (list)
        {
            list.Add(callback);
        }
    }

    /// <inheritdoc />
    public void UnregisterKeyHandler(string key, Action<bool> callback)
    {
        if (_handlers.TryGetValue(key, out var list))
        {
            lock (list)
            {
                list.Remove(callback);
            }
        }
    }

    /// <summary>
    /// Invoked from JavaScript on keydown with the <c>KeyboardEvent.key</c> value.
    /// </summary>
    /// <param name="key">The key identifier (exact string from the browser event).</param>
    [JSInvokable]
    public void OnKeyDown(string key)
    {
        if (_handlers.TryGetValue(key, out var list))
        {
            foreach (var cb in list.ToArray())
                cb(true);
        }
    }

    /// <summary>
    /// Invoked from JavaScript on keyup with the <c>KeyboardEvent.key</c> value.
    /// </summary>
    /// <param name="key">The key identifier (exact string from the browser event).</param>
    [JSInvokable]
    public void OnKeyUp(string key)
    {
        if (_handlers.TryGetValue(key, out var list))
        {
            foreach (var cb in list.ToArray())
                cb(false);
        }
    }

    /// <summary>
    /// Detaches JavaScript listeners and releases resources.
    /// </summary>
    /// <remarks>
    /// Swallows errors if the JS runtime is already disposed (e.g., during app shutdown).
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("inputService.unregister");
        }
        catch { /* swallow if runtime disposed */ }

        _objRef?.Dispose();

        GC.SuppressFinalize(this);
    }
}