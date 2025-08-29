using System;
using System.Net.Http.Headers;

namespace RTB.Blazor.UI.Extensions;

internal static class DictionaryExtension
{
    public static TValue GetValueOrDefault<TValue>(this IDictionary<string, object> dictionary, string key, TValue fallback = default!)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary), "Dictionary cannot be null.");
        }

        if (dictionary.TryGetValue(key, out var value) && value is TValue typedValue)
        {
            return typedValue;
        }

        return fallback;
    }

    public static IDictionary<string, object> Without(this IDictionary<string, object> dictionary, params string[] key)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary), "Dictionary cannot be null.");
        }

        if (key == null || key.Length == 0)
        {
            return dictionary;
        }

        var keysToRemove = new HashSet<string>(key, StringComparer.OrdinalIgnoreCase);
        return dictionary.Where(kvp => !keysToRemove.Contains(kvp.Key)).ToDictionary();
    }

    public static IDictionary<string, object> ByPrefix(this IDictionary<string, object> dictionary, string prefix, bool trimPrefix = true)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary), "Dictionary cannot be null.");
        }

        if (string.IsNullOrEmpty(prefix)) return dictionary;

        var pairsWithPrefix = dictionary.Where(kvp => kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        if (!pairsWithPrefix.Any()) return dictionary;
        if (!trimPrefix) return pairsWithPrefix.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Dictionary<string, object> pairsWithoutPrefix = new Dictionary<string, object>();

        foreach(var pair in pairsWithPrefix)
        {
            pairsWithoutPrefix.Add(pair.Key.Substring(prefix.Length), pair.Value);
        }

        return pairsWithoutPrefix;
    }
}
