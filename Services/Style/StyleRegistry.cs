using System;
using System.Collections.Concurrent;
using Microsoft.JSInterop;

namespace RTB.BlazorUI.Services.Style;

public interface IStyleRegistry
{
    string GetOrAdd(string css);
}

internal sealed class StyleRegistry(IJSRuntime jsRuntime) : IStyleRegistry
{
    private readonly ConcurrentDictionary<int, string> _cache = new();

    public string GetOrAdd(string css)
    {
        var hash = CssHasher.Hash(css);          // int hash
        if (_cache.TryAdd(hash, css))
        {
            // first time → emit style into <head>
            var className = $"s-{hash:X}";       // e.g. s-4B2CD7
            var rule      = $".{className}{{{css}}}";
            Inject(rule);
        }
        return $"s-{hash:X}";
    }

    private void Inject(string cssRule)
    {
        // WASM & CSR: use JS interop.
        // On prerendered Blazor Server you can also buffer IHtmlContent.
        jsRuntime.InvokeVoidAsync("rtbStyled.inject", cssRule);
    }
}
