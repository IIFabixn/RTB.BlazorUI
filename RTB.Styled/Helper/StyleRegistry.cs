using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;

namespace RTB.Styled.Helper;

public interface IStyleRegistry
{
    string GetOrCreate(string css);
    ValueTask InjectInto(string css, string className);
    ValueTask Remove(string className);
    ValueTask ClearAll();
}

public sealed class StyleRegistry(IJSRuntime jsRuntime) : IStyleRegistry
{
    // value is the original css (null until first InjectInto)
    private readonly ConcurrentDictionary<ulong, string?> _cache = new();

    public string GetOrCreate(string css)
    {
        var hash = CssHasher.Hash(css);
        var className = $"s-{hash:X}";

        // Hash-collision check
        if (_cache.TryGetValue(hash, out var existingCss) && existingCss is not null && existingCss != css)
        {
            // collision - derive a secondary hash
            hash = CssHasher.Hash($"{css}{css.Length}");
            className = $"s-{hash:X}"; // Update className with the new hash
        }

        _cache.TryAdd(hash, null);
        return className;
    }

    public ValueTask InjectInto(string css, string className)
    {
        if (string.IsNullOrEmpty(className)) throw new Exception("Can't inject stylings into class without class definition. className should not be empty!");
        if (string.IsNullOrWhiteSpace(css)) return ValueTask.CompletedTask;

        var classHash = className[2..]; // strip "s-" prefix
        if (!ulong.TryParse(classHash, System.Globalization.NumberStyles.HexNumber, null, out var hashValue))
        {
            throw new ArgumentException($"Invalid className format: {className}. Expected format is 's-<hexadecimal>'.", nameof(className));
        }

        if (_cache.ContainsKey(hashValue))
        {
            _cache[hashValue] = css; // realize the cache

            return jsRuntime.InvokeVoidAsync("rtbStyled.injectInto", css, className);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Remove(string className)
    {
        var ch = className[2..]; // strip "s-" prefix
        if (!ulong.TryParse(ch, System.Globalization.NumberStyles.HexNumber, null, out var hashValue))
        {
            throw new ArgumentException($"Invalid className format: {className}. Expected format is 's-<hexadecimal>'.", nameof(className));
        }

        if (_cache.TryRemove(hashValue, out _))
        {
            return jsRuntime.InvokeVoidAsync("rtbStyled.clearRule", className);
        }

        return ValueTask.CompletedTask; // No-op if the className was not found in the cache
    }

    public ValueTask ClearAll()
    {
        // Clear the cache
        _cache.Clear();
        // Purge <style> content completely
        return jsRuntime.InvokeVoidAsync("rtbStyled.clear");
    }
}
