using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Styled.Helper;

public static class CssEnumExtensions
{
    public static string ToCss(this Enum e)
    {
        var src = e.ToString();

        // count added hyphens
        int extra = src.Count(char.IsUpper) - 1; // one per word break
        int len = src.Length + Math.Max(extra, 0);

        return string.Create(len, src, (span, value) =>
        {
            var pos = 0;
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (char.IsUpper(c))
                {
                    if (i != 0) span[pos++] = '-';
                    c = char.ToLowerInvariant(c);
                }
                span[pos++] = c;
            }
        });
    }
}
