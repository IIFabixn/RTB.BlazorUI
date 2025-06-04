using System;

namespace RTB.BlazorUI.Services.Style;

internal static class CssHasher
{
    // FNV-1a 32-bit
    public static int Hash(string s)
    {
        const uint fnvPrime = 0x01000193;
        uint hash = 0x811C9DC5;
        foreach (var ch in s)
            hash = (hash ^ ch) * fnvPrime;
        return unchecked((int)hash);
    }
}
