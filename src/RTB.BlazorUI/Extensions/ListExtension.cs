using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.UI.Extensions
{
    internal static class ListExtension
    {
        public static bool TryGetItem<TValue>(this List<TValue> list, Func<TValue, bool> condition, out TValue item)
        {
            item = default!;
            if (list == null || list.Count == 0)
                return false;
            var foundItem = list.FirstOrDefault(condition);
            if (foundItem != null)
            {
                item = foundItem;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<TValue> GetRange<TValue>(this IEnumerable<TValue> source, TValue start, TValue end)
        {
            ArgumentNullException.ThrowIfNull(source);
            if (start == null || end == null)
                yield break;

            var list = source.ToList();
            var startIndex = list.IndexOf(start);
            var endIndex = list.IndexOf(end);

            if (startIndex == -1 || endIndex == -1 || startIndex > endIndex)
                yield break;

            for (int i = startIndex; i <= endIndex; i++)
            {
                yield return list.ElementAt(i);
            }
        }
    }
}
