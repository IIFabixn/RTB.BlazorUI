using System;
using System.Collections.Concurrent;
using Microsoft.JSInterop;

namespace RTB.BlazorUI.Services.Style;

public interface IStyleRegistry
{
    string Create();
    string GetOrAdd(string css);
    void InjectInto(string css, string className);
    void Clear();
}

internal sealed class StyleRegistry(IJSRuntime jsRuntime) : IStyleRegistry
{
    private readonly ConcurrentDictionary<int, string> _cache = new();
    private readonly Queue<string> _pendingRules = new();
    private readonly Lock _lock = new();
    private bool _injectionScheduled = false;

    public string Create()
    {
        // Generate a new unique hash for an empty CSS rule
        var timestamp = DateTime.UtcNow.Ticks;
        var hash = CssHasher.Hash(timestamp.ToString());
        var className = $"s-{hash:X}";

        // Add to cache with empty CSS
        if (_cache.TryAdd(hash, string.Empty))
        {
            lock(_lock) {
                // Create empty CSS rule and schedule injection
                var rule = $".{className}{{}}";
                _pendingRules.Append(rule);
                _ = Task.Run(FlushPendingRules);
            }
        }
        
        return className;
    }

    public string GetOrAdd(string css)
    {
        var hash = CssHasher.Hash(css);
        if (_cache.ContainsKey(hash)) return $"s-{hash:X}";

        if (_cache.TryAdd(hash, css))
        {
            var className = $"s-{hash:X}";
            var rule = $".{className}{{{css}}}";

            lock (_lock)
            {
                _pendingRules.Enqueue(rule);
                if (!_injectionScheduled)
                {
                    _injectionScheduled = true;
                    _ = Task.Run(FlushPendingRules);
                }
            }
        }

        return $"s-{hash:X}";
    }

    private async Task FlushPendingRules()
    {
        await Task.Yield(); // Wait for render cycle to complete
        lock (_lock)
        {
            while (_pendingRules.Count > 0)
            {
                jsRuntime.InvokeVoidAsync("rtbStyled.inject", _pendingRules.Dequeue());
            }
            _injectionScheduled = false;
        }
    }

    public void InjectInto(string css, string className)
    {
        if (string.IsNullOrWhiteSpace(css) || string.IsNullOrWhiteSpace(className))
            return;
        
        var hash = className.Substring(2); // Remove "s-" prefix
        var hashValue = int.Parse(hash, System.Globalization.NumberStyles.HexNumber);

        if (_cache.TryGetValue(hashValue, out var value))
        {
            // Use the JavaScript injectInto method to add CSS to existing class
            jsRuntime.InvokeVoidAsync("rtbStyled.injectInto", css, className);
        }
    }

    public void Clear()
    {
        // Clear the cache
        _cache.Clear();
        // Clear the styles in the document head
        jsRuntime.InvokeVoidAsync("rtbStyled.clear");
    }
}
