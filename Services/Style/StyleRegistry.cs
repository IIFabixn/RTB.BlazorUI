using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;

namespace RTB.BlazorUI.Services.Style;

public interface IStyleRegistry
{
    Task<string> GetOrCreate(string css);
    Task InjectInto(string css, string className);
    Task Clear();
}

internal sealed class StyleRegistry(IJSRuntime jsRuntime) : IStyleRegistry
{
    private readonly ConcurrentDictionary<int, string> _cache = new();

    public async Task<string> GetOrCreate(string css)
    {
        var hash = CssHasher.Hash(css);
        var className = $"s-{hash:X}";

        // Add to cache with empty CSS
        if (_cache.TryAdd(hash, string.Empty))
        {
            // Create empty CSS rule and schedule injection
            await jsRuntime.InvokeVoidAsync("rtbStyled.injectInto", css, className).ConfigureAwait(false);
        }
        
        return className;
    }

    public async Task InjectInto(string css, string className)
    {
        if (string.IsNullOrEmpty(className)) throw new Exception("Can't inject stylings into class without class definition. className should not be empty!");
        if (string.IsNullOrWhiteSpace(css)) return;

        var classHash = className.Substring(2); // Remove "s-" prefix
        var hashValue = int.Parse(classHash, System.Globalization.NumberStyles.HexNumber);

        if (_cache.ContainsKey(hashValue))
        {
            _cache[hashValue] = css;

            await jsRuntime.InvokeVoidAsync("rtbStyled.injectInto", css, className).ConfigureAwait(false);
        }
    }

    public async Task Clear()
    {
        // Clear the cache
        _cache.Clear();
        // Clear the styles in the document head
        await jsRuntime.InvokeVoidAsync("rtbStyled.clear");
    }
}
