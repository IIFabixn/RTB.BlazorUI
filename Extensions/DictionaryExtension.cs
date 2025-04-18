using System;

namespace RTB.BlazorUI.Extensions;

public static class DictionaryExtension
{
    public static IEnumerable<KeyValuePair<string, TValue>> Without<TValue>(this IDictionary<string, TValue> dictionary, params string[] key)
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
