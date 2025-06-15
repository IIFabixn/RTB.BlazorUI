using System;

namespace RTB.BlazorUI.Services.Style;

internal static class CssHasher
{
    // FNV-1a 64-bit
    public static ulong Hash(string s)
    {
        const ulong prime = 1099511628211UL;
        ulong hash = 14695981039346656037UL;
        foreach (var ch in s)
            hash = (hash ^ (byte)ch) * prime;
        return hash;
    }
}
