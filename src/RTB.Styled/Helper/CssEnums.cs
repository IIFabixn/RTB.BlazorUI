using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper;

/// <summary>
/// CSS related enums and extensions.
/// </summary>
public enum Unit 
{
    /// <summary>
    /// Pixels
    /// </summary>
    Px,
    /// <summary>
    /// Root em
    /// </summary>
    Rem,
    /// <summary>
    /// Em
    /// </summary>
    Em,
    /// <summary>
    /// Percent
    /// </summary>
    Percent,
    /// <summary>
    /// Viewport width
    /// </summary>
    Vw,
    /// <summary>
    /// Viewport height
    /// </summary>
    Vh
}

/// <summary>
/// CSS place enum.
/// </summary>
public enum Place 
{
    /// <summary>
    /// Start (flex-start)
    /// </summary>
    Start,
    /// <summary>
    /// End (flex-end)
    /// </summary>
    End,
    /// <summary>
    /// Center
    /// </summary>
    Center,
    /// <summary>
    /// Stretch
    /// </summary>
    Stretch
}

/// <summary>
/// CSS enum extensions.
/// </summary>
public static class CssEnumExtensions
{
    /// <summary>
    /// Converts an enum value to a CSS-compatible string (kebab-case).
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
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
