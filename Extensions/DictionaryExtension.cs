using System;
using System.Net.Http.Headers;

namespace RTB.BlazorUI.Extensions;

public static class DictionaryExtension
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

    public static IEnumerable<KeyValuePair<string, object>> Without(this IDictionary<string, object> dictionary, params string[] key)
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
        return dictionary.Where(kvp => !keysToRemove.Contains(kvp.Key));
    }
}
