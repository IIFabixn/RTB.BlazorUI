using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Services;

public interface IStyleRegistry
{
    string GetOrCreate(string css);
    ValueTask InjectInto(string css, string className);
    ValueTask<bool> TryRemove(string className);
    ValueTask ClearAll();
}

public sealed class StyleRegistry(IJSRuntime jsRuntime) : IStyleRegistry
{
    // value is the original css (null until first InjectInto)
    private readonly ConcurrentDictionary<ulong, int> _cache = new();
        
    public string GetOrCreate(string css)
    {
        var hash = CssHasher.Hash(css);
        var className = $"s-{hash:X}";

        // Hash-collision check
        if (_cache.TryGetValue(hash, out int value))
        {
            // using the same class twice
            _cache[hash] = ++value;
        }
        else
        {
            // first accourence
            _cache.TryAdd(hash, 1);        
        }

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
            return jsRuntime.InvokeVoidAsync("rtbStyled.injectInto", css, className);
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask<bool> TryRemove(string className)
    {
        if (string.IsNullOrEmpty(className)) return false;

        var ch = className[2..]; // strip "s-" prefix
        if (!ulong.TryParse(ch, System.Globalization.NumberStyles.HexNumber, null, out var hashValue))
        {
            throw new ArgumentException($"Invalid className format: {className}. Expected format is 's-<hexadecimal>'.", nameof(className));
        }

        if (_cache.TryGetValue(hashValue, out var applience))
        {
            _cache[hashValue] = --applience;
            if (applience <= 0)
            {
                await jsRuntime.InvokeVoidAsync("rtbStyled.clearRule", className);
                return true;
            }
        }

        return false;
    }

    public ValueTask ClearAll()
    {
        // Clear the cache
        _cache.Clear();
        // Purge <style> content completely
        return jsRuntime.InvokeVoidAsync("rtbStyled.clear");
    }
}
