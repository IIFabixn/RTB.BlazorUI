using Microsoft.JSInterop;
using System.Collections.Concurrent;

public interface IInputService : IAsyncDisposable
{
    /// <summary>
    /// Subscribe to a specific key (like "Shift", "Control", "a", etc.).
    /// </summary>
    /// <param name="key">The key string as reported by KeyboardEvent.key</param>
    /// <param name="callback">Callback that receives true on key down, false on key up</param>
    void RegisterKeyHandler(string key, Action<bool> callback);

    /// <summary>
    /// Unsubscribe a handler.
    /// </summary>
    void UnregisterKeyHandler(string key, Action<bool> callback);

    /// <summary>
    /// Initialize JS event listeners once.
    /// </summary>
    Task InitializeAsync();
}

public class InputService(IJSRuntime jsRuntime) : IInputService
{
    private DotNetObjectReference<InputService>? _objRef;
    
    private readonly ConcurrentDictionary<string, List<Action<bool>>> _handlers = new();

    public async Task InitializeAsync()
    {
        _objRef = DotNetObjectReference.Create(this);
        await jsRuntime.InvokeVoidAsync("inputService.register", _objRef);
    }

    public void RegisterKeyHandler(string key, Action<bool> callback)
    {
        var list = _handlers.GetOrAdd(key, _ => []);
        lock (list)
        {
            list.Add(callback);
        }
    }

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

    [JSInvokable]
    public void OnKeyDown(string key)
    {
        if (_handlers.TryGetValue(key, out var list))
        {
            foreach (var cb in list.ToArray())
                cb(true);
        }
    }

    [JSInvokable]
    public void OnKeyUp(string key)
    {
        if (_handlers.TryGetValue(key, out var list))
        {
            foreach (var cb in list.ToArray())
                cb(false);
        }
    }

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