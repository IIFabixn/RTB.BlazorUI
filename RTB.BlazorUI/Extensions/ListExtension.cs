using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Extensions
{
    public static class ListExtension
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
    }
}
