using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Services;

/// <summary>
/// Registry for managing CSS styles with reference counting and injection into the document.
/// </summary>
public interface IStyleRegistry
{
    /// <summary>
    /// Gets or creates a unique class name for the given CSS.
    /// </summary>
    /// <param name="css"></param>
    /// <returns></returns>
    string GetOrCreate(string css);

    /// <summary>
    /// Injects the given CSS into the document under the specified class name.
    /// </summary>
    /// <param name="css"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    ValueTask InjectInto(string css, string className);

    /// <summary>
    /// Tries to remove the style associated with the given class name.
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    ValueTask<bool> TryRemove(string className);

    /// <summary>
    /// Clears all styles from the registry and the document.
    /// </summary>
    /// <returns></returns>
    ValueTask ClearAll();
}

public sealed class StyleRegistry(IJSRuntime jsRuntime) : IStyleRegistry
{
    // value is the original css (null until first InjectInto)
    private readonly ConcurrentDictionary<ulong, Entry> _entries = new();

    /// <summary>
    /// Reference count and emission state of a style entry.
    /// </summary>
    private sealed record Entry
    {
        /// <summary>
        /// Number of active references to this style.
        /// </summary>
        public int RefCount;

        /// <summary>
        /// 0 = not emitted, 1 = emitting, 2 = emitted
        /// </summary>
        public int Emission;
    }

    /// <summary>
    /// Gets or creates a unique class name for the given CSS.
    /// </summary>
    /// <param name="css"></param>
    /// <returns></returns>
    public string GetOrCreate(string css)
    {
        var hash = CssHasher.Hash(css);
        var entry = _entries.AddOrUpdate(
            hash,
            _ => new Entry { RefCount = 1, Emission = 0 },
            (_, e) => { Interlocked.Increment(ref e.RefCount); return e; });

        return $"s-{hash:X}";
    }

    /// <summary>
    /// Injects the given CSS into the document under the specified class name.
    /// </summary>
    /// <param name="css"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentException"></exception>
    public ValueTask InjectInto(string css, string className)
    {
        if (string.IsNullOrEmpty(className))
            throw new Exception("Can't inject stylings into class without class definition. className should not be empty!");

        if (string.IsNullOrWhiteSpace(css))
            return ValueTask.CompletedTask;

        if (!TryParseHash(className, out var hash))
            throw new ArgumentException($"Invalid className format: {className}. Expected 's-<hex>'.", nameof(className));

        if (!_entries.TryGetValue(hash, out var entry))
            return ValueTask.CompletedTask; // no refs -> nothing to do

        // Injection barrier: only the winner injects (0 -> 1)
        if (Interlocked.CompareExchange(ref entry.Emission, 1, 0) == 0)
        {
            return EmitAsync(css, className, entry);
        }

        // someone else is injecting or already injected
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Performs the actual injection of CSS into the document.
    /// </summary>
    /// <param name="css"></param>
    /// <param name="className"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    private async ValueTask EmitAsync(string css, string className, Entry entry)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("rtbStyled.injectInto", css, className);
        }
        finally
        {
            Volatile.Write(ref entry.Emission, 2); // mark as injected
        }
    }

    /// <summary>
    /// Tries to remove the style associated with the given class name.
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async ValueTask<bool> TryRemove(string className)
    {
        if (string.IsNullOrEmpty(className)) return false;
        if (!TryParseHash(className, out var hash))
            throw new ArgumentException($"Invalid className format: {className}. Expected 's-<hex>'.", nameof(className));

        if (!_entries.TryGetValue(hash, out var entry)) return false;

        // decrement refcount atomically
        while (true)
        {
            var current = Volatile.Read(ref entry.RefCount);
            if (current <= 0) return false;
            if (Interlocked.CompareExchange(ref entry.RefCount, current - 1, current) == current)
                break;
        }

        if (Volatile.Read(ref entry.RefCount) > 0) return false;

        // last user: remove entry and clear rule
        _entries.TryRemove(hash, out _);

        // If injection is in-flight, clearing is still safe: JS can no-op if rule not present yet
        await jsRuntime.InvokeVoidAsync("rtbStyled.clearRule", className);
        return true;
    }

    /// <summary>
    /// Clears all styles from the registry and the document.
    /// </summary>
    /// <returns></returns>
    public ValueTask ClearAll()
    {
        _entries.Clear();
        return jsRuntime.InvokeVoidAsync("rtbStyled.clearAll");
    }

    /// <summary>
    /// Parses the hash from the class name.
    /// </summary>
    /// <param name="className"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    private static bool TryParseHash(string className, out ulong hash)
    {
        var span = className.AsSpan();
        if (span.Length < 3 || span[0] != 's' || span[1] != '-') { hash = 0; return false; }
        var hex = span[2..].ToString();
        return ulong.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out hash);
    }
}
