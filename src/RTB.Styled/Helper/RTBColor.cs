using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RTB.Blazor.Styled.Helper;

/// <summary>
/// Represents a color in the sRGB RGBA color space (8-bit per channel).
/// Aligns with common CSS color syntaxes per MDN (hex, rgb[a], hsl[a], named).
/// </summary>
public readonly partial record struct RTBColor(byte R, byte G, byte B, byte A)
{
    #region Constructors & basic factories

    public static RTBColor FromRgb(byte r, byte g, byte b) => new(r, g, b, 255);
    public static RTBColor FromRgba(byte r, byte g, byte b, byte a) => new(r, g, b, a);

    #endregion

    #region Properties

    public byte Alpha => A;
    public byte Red => R;
    public byte Green => G;
    public byte Blue => B;

    public string HexRgb => $"#{R:X2}{G:X2}{B:X2}";
    public string HexRgba => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    public override string ToString() => HexRgba;

    #endregion

    #region Parsing (CSS-aligned)

    /// <summary>Parse a CSS color value. Supports hex, rgb[a], hsl[a], named colors, and 'transparent'.</summary>
    public static RTBColor Parse(string css)
    {
        if (TryParse(css, out var c)) return c;
        throw new FormatException($"Unrecognized color format '{css}'.");
    }

    /// <summary>Try to parse a CSS color value safely.</summary>
    public static bool TryParse(string? css, out RTBColor color)
    {
        color = default;

        if (string.IsNullOrWhiteSpace(css)) return false;
        css = css.Trim();

        // Hex shortcuts first
        if (css[0] == '#')
        {
            return TryParseHex(css, out color);
        }

        // functional notations
        if (StartsWithIdent(css, "rgb(") || StartsWithIdent(css, "rgba("))
            return TryParseRgbFunction(css, out color);

        if (StartsWithIdent(css, "hsl(") || StartsWithIdent(css, "hsla("))
            return TryParseHslFunction(css, out color);

        // named colors & transparent
        if (css.Equals("transparent", StringComparison.OrdinalIgnoreCase))
        {
            color = new RTBColor(0, 0, 0, 0);
            return true;
        }

        if (CssNamedColors.TryGetValue(css, out var named))
        {
            color = named;
            return true;
        }

        return false;
    }

    public static RTBColor FromCss(string css) => Parse(css);

    public static implicit operator RTBColor(string css) => Parse(css);
    public static implicit operator string(RTBColor c) => c.HexRgba;

    private static bool StartsWithIdent(string text, string prefix) =>
        text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

    #endregion

    #region Hex parsing

    private static bool TryParseHex(string hex, out RTBColor color)
    {
        color = default;
        var s = hex.StartsWith('#') ? hex[1..] : hex;
        try
        {
            switch (s.Length)
            {
                case 3: // #RGB
                    {
                        byte r = (byte)(Hex1(s[0]) * 17);
                        byte g = (byte)(Hex1(s[1]) * 17);
                        byte b = (byte)(Hex1(s[2]) * 17);
                        color = new RTBColor(r, g, b, 255);
                        return true;
                    }
                case 4: // #RGBA
                    {
                        byte r = (byte)(Hex1(s[0]) * 17);
                        byte g = (byte)(Hex1(s[1]) * 17);
                        byte b = (byte)(Hex1(s[2]) * 17);
                        byte a = (byte)(Hex1(s[3]) * 17);
                        color = new RTBColor(r, g, b, a);
                        return true;
                    }
                case 6: // #RRGGBB
                    {
                        byte r = Hex2(s.AsSpan(0, 2));
                        byte g = Hex2(s.AsSpan(2, 2));
                        byte b = Hex2(s.AsSpan(4, 2));
                        color = new RTBColor(r, g, b, 255);
                        return true;
                    }
                case 8: // #RRGGBBAA
                    {
                        byte r = Hex2(s.AsSpan(0, 2));
                        byte g = Hex2(s.AsSpan(2, 2));
                        byte b = Hex2(s.AsSpan(4, 2));
                        byte a = Hex2(s.AsSpan(6, 2));
                        color = new RTBColor(r, g, b, a);
                        return true;
                    }
                default:
                    return false;
            }
        }
        catch
        {
            return false;
        }

        static byte Hex1(char c) => Convert.ToByte(c.ToString(), 16);
        static byte Hex2(ReadOnlySpan<char> s) => Convert.ToByte(s.ToString(), 16);
    }

    #endregion

    #region rgb()/rgba() parsing

    // Accepts:
    //   rgb(255, 0, 0)
    //   rgba(255,0,0,0.5)
    //   rgb(100% 0% 0% / 50%)
    //   rgb(255 0 0 / 1)
    //   rgb(255 0 0)    (alpha=1)
    private static readonly Regex RgbFunc =
        RgbRegex();

    private static bool TryParseRgbFunction(string input, out RTBColor color)
    {
        color = default;
        var m = RgbFunc.Match(input);
        if (!m.Success) return false;

        var inner = m.Groups[1].Value.Trim();

        string[] comps;
        string? alphaPart = null;

        if (inner.Contains(',')) // legacy comma-separated: rgb(a, b, c[, a])
        {
            var parts = inner.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length is not (3 or 4)) return false;

            // If there are 4 parts, last one is alpha
            if (parts.Length == 4)
            {
                alphaPart = parts[3];
            }
            comps = [parts[0], parts[1], parts[2]];
        }
        else
        {
            // modern space-separated with optional " / alpha"
            string compsPart, alphaTemp = null!;
            var slashIdx = inner.IndexOf('/');
            if (slashIdx >= 0)
            {
                compsPart = inner[..slashIdx].Trim();
                alphaTemp = inner[(slashIdx + 1)..].Trim();
            }
            else
            {
                compsPart = inner;
            }

            var list = SplitCssList(compsPart, expected: 3);
            if (list.Count != 3) return false;

            comps = list.ToArray();
            alphaPart = slashIdx >= 0 ? alphaTemp : null;
        }

        if (!TryParseRgbComponent(comps[0], out var r)) return false;
        if (!TryParseRgbComponent(comps[1], out var g)) return false;
        if (!TryParseRgbComponent(comps[2], out var b)) return false;

        byte a = 255;
        if (alphaPart is not null)
        {
            if (!TryParseAlpha(alphaPart, out a)) return false;
        }

        color = new RTBColor(r, g, b, a);
        return true;
    }

    private static bool TryParseRgbComponent(string token, out byte value)
    {
        token = token.Trim();
        if (token.EndsWith('%'))
        {
            if (!TryParseDouble(token[..^1], out var pct)) { value = 0; return false; }
            pct = Clamp01(pct / 100.0);
            value = (byte)Math.Round(pct * 255.0);
            return true;
        }
        else
        {
            // integer 0..255
            if (!TryParseDouble(token, out var n)) { value = 0; return false; }
            n = Math.Clamp(Math.Round(n), 0, 255);
            value = (byte)n;
            return true;
        }
    }

    #endregion

    #region hsl()/hsla() parsing

    // Accepts:
    //   hsl(120, 100%, 50%)
    //   hsla(120,100%,50%,0.25)
    //   hsl(120 100% 50% / 25%)
    // Hue may be a number or with 'deg'. (For simplicity we treat number == deg)
    private static readonly Regex HslFunc =
        HslRegex();

    private static bool TryParseHslFunction(string input, out RTBColor color)
    {
        color = default;
        var m = HslFunc.Match(input);
        if (!m.Success) return false;

        var inner = m.Groups[1].Value.Trim();

        string[] comps;
        string? alphaPart = null;

        if (inner.Contains(',')) // legacy comma-separated: hsl(h, s%, l%) / hsla(h, s%, l%, a)
        {
            var parts = inner.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length is not (3 or 4)) return false;

            if (parts.Length == 4) alphaPart = parts[3];
            comps = [parts[0], parts[1], parts[2]];
        }
        else
        {
            // modern space-separated with optional " / alpha"
            string compsPart, alphaTemp = null!;
            var slashIdx = inner.IndexOf('/');
            if (slashIdx >= 0)
            {
                compsPart = inner[..slashIdx].Trim();
                alphaTemp = inner[(slashIdx + 1)..].Trim();
            }
            else
            {
                compsPart = inner;
            }

            var list = SplitCssList(compsPart, expected: 3);
            if (list.Count != 3) return false;

            comps = [.. list];
            alphaPart = slashIdx >= 0 ? alphaTemp : null;
        }

        if (!TryParseHue(comps[0], out var h)) return false;
        if (!TryParsePercent01(comps[1], out var s)) return false;
        if (!TryParsePercent01(comps[2], out var l)) return false;

        byte a = 255;
        if (alphaPart is not null)
        {
            if (!TryParseAlpha(alphaPart, out a)) return false;
        }

        color = HslToColor(h, s, l, a);
        return true;
    }

    private static bool TryParseHue(string token, out double hue01)
    {
        token = token.Trim().ToLowerInvariant();
        token = token.EndsWith("deg") ? token[..^3].Trim() : token;
        if (!TryParseDouble(token, out var deg)) { hue01 = 0; return false; }
        // Wrap into [0,360)
        deg %= 360.0;
        if (deg < 0) deg += 360.0;
        hue01 = deg / 360.0;
        return true;
    }

    private static bool TryParsePercent01(string token, out double value01)
    {
        token = token.Trim();
        if (!token.EndsWith('%')) { value01 = 0; return false; }
        if (!TryParseDouble(token[..^1], out var pct)) { value01 = 0; return false; }
        value01 = Clamp01(pct / 100.0);
        return true;
    }

    #endregion

    #region Alpha parsing & utilities

    // alpha can be a number 0..1 or percentage 0%..100%
    private static bool TryParseAlpha(string token, out byte a)
    {
        token = token.Trim();
        if (token.EndsWith('%'))
        {
            if (!TryParseDouble(token[..^1], out var pct)) { a = 255; return false; }
            pct = Clamp01(pct / 100.0);
            a = (byte)Math.Round(pct * 255.0);
            return true;
        }
        else
        {
            if (!TryParseDouble(token, out var n)) { a = 255; return false; }
            n = Clamp01(n);
            a = (byte)Math.Round(n * 255.0);
            return true;
        }
    }

    private static bool TryParseDouble(string s, out double d) =>
        double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d);

    private static string NormalizeCommasToSpaces(string s) =>
        s.Replace(",", " ", StringComparison.Ordinal);

    private static List<string> SplitCssList(string s, int expected)
    {
        var parts = new List<string>(expected);
        foreach (var piece in s.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries))
            parts.Add(piece);
        return parts;
    }

    private static double Clamp01(double v) => v < 0 ? 0 : v > 1 ? 1 : v;

    #endregion

    #region Color math (HSL conversion & adjustments)

    public RTBColor WithAlpha(double alpha) => new(R, G, B, (byte)(Clamp01(alpha) * 255));

    public RTBColor Lighten(double pct) => HslShift(L: pct);
    public RTBColor Darken(double pct) => HslShift(L: -pct);
    public RTBColor Saturate(double pct) => HslShift(S: pct);
    public RTBColor Desaturate(double pct) => HslShift(S: -pct);

    public RTBColor Blend(RTBColor target, double pct)
    {
        pct = Clamp01(pct);
        byte lerp(byte a, byte b) => (byte)(a + (b - a) * pct);
        return new(
            lerp(R, target.R),
            lerp(G, target.G),
            lerp(B, target.B),
            lerp(A, target.A));
    }

    private static double Clamp01(byte v) => v / 255.0;

    private RTBColor HslShift(double H = 0, double S = 0, double L = 0)
    {
        ColorToHsl(this, out var h, out var s, out var l);
        h = ((h + H) % 1 + 1) % 1; // wrap into [0,1)
        s = Math.Clamp(s + S, 0, 1);
        l = Math.Clamp(l + L, 0, 1);
        return HslToColor(h, s, l, A);
    }

    private static void ColorToHsl(RTBColor color, out double hue, out double satuation, out double luminance)
    {
        double r = color.R / 255.0, g = color.G / 255.0, b = color.B / 255.0;
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        luminance = (max + min) / 2.0;

        if (Math.Abs(max - min) < 1e-6) { hue = satuation = 0; return; }

        double d = max - min;
        satuation = luminance > .5 ? d / (2 - max - min) : d / (max + min);

        if (Math.Abs(max - r) < 1e-9)
            hue = ((g - b) / d + (g < b ? 6 : 0)) / 6.0;
        else if (Math.Abs(max - g) < 1e-9)
            hue = ((b - r) / d + 2) / 6.0;
        else
            hue = ((r - g) / d + 4) / 6.0;
    }

    private static RTBColor HslToColor(double hue, double satuation, double luminance, byte alpha)
    {
        if (satuation <= 1e-6)
        {
            byte v = (byte)Math.Round(luminance * 255.0);
            return new RTBColor(v, v, v, alpha);
        }

        double q = luminance < .5 ? luminance * (1 + satuation) : luminance + satuation - luminance * satuation;
        double p = 2 * luminance - q;

        double ToRgb(double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }

        byte r = (byte)Math.Round(ToRgb(hue + 1.0 / 3) * 255.0);
        byte g = (byte)Math.Round(ToRgb(hue) * 255.0);
        byte b = (byte)Math.Round(ToRgb(hue - 1.0 / 3) * 255.0);
        return new RTBColor(r, g, b, alpha);
    }

    [GeneratedRegex(@"^hsla?\((.+)\)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex HslRegex();
    [GeneratedRegex(@"^rgba?\((.+)\)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex RgbRegex();

    #endregion
}

/// <summary>
/// Common named CSS colors (CSS Color Module Level 4).
/// </summary>
public static class RTBColors
{
    // Keep your previous convenience names (backward compatible)
    public static RTBColor White => "#FFFFFFFF";
    public static RTBColor Gray => "#808080FF";
    public static RTBColor Black => "#000000FF";

    public static RTBColor Yellow => "#FFFF00FF";
    public static RTBColor Red => "#FF0000FF";
    public static RTBColor Blue => "#0000FFFF";
    public static RTBColor Orange => "#FF8800FF";
    public static RTBColor Green => "#00FF00FF";
    public static RTBColor Magenta => "#FF00FFFF";

    public static RTBColor Transparent => "#00000000";

    // Full CSS named colors as dictionary (lowercase keys)
    public static IReadOnlyDictionary<string, RTBColor> Named => CssNamedColors.Map;

    // Helper to access a named color directly
    public static RTBColor NamedColor(string name) =>
        CssNamedColors.TryGetValue(name, out var c) ? c : throw new KeyNotFoundException($"Unknown CSS color name '{name}'.");
}

/// <summary>
/// Storage for CSS Level 4 named colors (lowercase keys).
/// </summary>
internal static class CssNamedColors
{
    public static readonly IReadOnlyDictionary<string, RTBColor> Map = Build();

    public static bool TryGetValue(string name, out RTBColor color) =>
        Map.TryGetValue(name.Trim().ToLowerInvariant(), out color);

    private static IReadOnlyDictionary<string, RTBColor> Build()
    {
        // Source: MDN / CSS Color Module Level 4.
        // (List abbreviated for brevity—add more as needed.)
        var d = new Dictionary<string, RTBColor>(StringComparer.OrdinalIgnoreCase)
        {
            ["black"] = "#000000FF",
            ["silver"] = "#C0C0C0FF",
            ["gray"] = "#808080FF",
            ["white"] = "#FFFFFFFF",
            ["maroon"] = "#800000FF",
            ["red"] = "#FF0000FF",
            ["purple"] = "#800080FF",
            ["fuchsia"] = "#FF00FFFF",
            ["green"] = "#008000FF",
            ["lime"] = "#00FF00FF",
            ["olive"] = "#808000FF",
            ["yellow"] = "#FFFF00FF",
            ["navy"] = "#000080FF",
            ["blue"] = "#0000FFFF",
            ["teal"] = "#008080FF",
            ["aqua"] = "#00FFFFFF",

            ["orange"] = "#FFA500FF",
            ["aliceblue"] = "#F0F8FFFF",
            ["antiquewhite"] = "#FAEBD7FF",
            ["aquamarine"] = "#7FFFD4FF",
            ["azure"] = "#F0FFFFFF",
            ["beige"] = "#F5F5DCFF",
            ["bisque"] = "#FFE4C4FF",
            ["blanchedalmond"] = "#FFEBCDFF",
            ["blueviolet"] = "#8A2BE2FF",
            ["brown"] = "#A52A2AFF",
            ["burlywood"] = "#DEB887FF",
            ["cadetblue"] = "#5F9EA0FF",
            ["chartreuse"] = "#7FFF00FF",
            ["chocolate"] = "#D2691EFF",
            ["coral"] = "#FF7F50FF",
            ["cornflowerblue"] = "#6495EDFF",
            ["cornsilk"] = "#FFF8DCFF",
            ["crimson"] = "#DC143CFF",
            ["cyan"] = "#00FFFFFF",
            ["darkblue"] = "#00008BFF",
            ["darkcyan"] = "#008B8BFF",
            ["darkgoldenrod"] = "#B8860BFF",
            ["darkgray"] = "#A9A9A9FF",
            ["darkgreen"] = "#006400FF",
            ["darkgrey"] = "#A9A9A9FF",
            ["darkkhaki"] = "#BDB76BFF",
            ["darkmagenta"] = "#8B008BFF",
            ["darkolivegreen"] = "#556B2FFF",
            ["darkorange"] = "#FF8C00FF",
            ["darkorchid"] = "#9932CCFF",
            ["darkred"] = "#8B0000FF",
            ["darksalmon"] = "#E9967AFF",
            ["darkseagreen"] = "#8FBC8FFF",
            ["darkslateblue"] = "#483D8BFF",
            ["darkslategray"] = "#2F4F4FFF",
            ["darkslategrey"] = "#2F4F4FFF",
            ["darkturquoise"] = "#00CED1FF",
            ["darkviolet"] = "#9400D3FF",
            ["deeppink"] = "#FF1493FF",
            ["deepskyblue"] = "#00BFFFFF",
            ["dimgray"] = "#696969FF",
            ["dimgrey"] = "#696969FF",
            ["dodgerblue"] = "#1E90FFFF",
            ["firebrick"] = "#B22222FF",
            ["floralwhite"] = "#FFFAF0FF",
            ["forestgreen"] = "#228B22FF",
            ["gainsboro"] = "#DCDCDCFF",
            ["ghostwhite"] = "#F8F8FFFF",
            ["gold"] = "#FFD700FF",
            ["goldenrod"] = "#DAA520FF",
            ["greenyellow"] = "#ADFF2FFF",
            ["grey"] = "#808080FF",
            ["honeydew"] = "#F0FFF0FF",
            ["hotpink"] = "#FF69B4FF",
            ["indianred"] = "#CD5C5CFF",
            ["indigo"] = "#4B0082FF",
            ["ivory"] = "#FFFFF0FF",
            ["khaki"] = "#F0E68CFF",
            ["lavender"] = "#E6E6FAFF",
            ["lavenderblush"] = "#FFF0F5FF",
            ["lawngreen"] = "#7CFC00FF",
            ["lemonchiffon"] = "#FFFACDFF",
            ["lightblue"] = "#ADD8E6FF",
            ["lightcoral"] = "#F08080FF",
            ["lightcyan"] = "#E0FFFFFF",
            ["lightgoldenrodyellow"] = "#FAFAD2FF",
            ["lightgray"] = "#D3D3D3FF",
            ["lightgreen"] = "#90EE90FF",
            ["lightgrey"] = "#D3D3D3FF",
            ["lightpink"] = "#FFB6C1FF",
            ["lightsalmon"] = "#FFA07AFF",
            ["lightseagreen"] = "#20B2AAFF",
            ["lightskyblue"] = "#87CEFAFF",
            ["lightslategray"] = "#778899FF",
            ["lightslategrey"] = "#778899FF",
            ["lightsteelblue"] = "#B0C4DEFF",
            ["lightyellow"] = "#FFFFE0FF",
            ["limegreen"] = "#32CD32FF",
            ["linen"] = "#FAF0E6FF",
            ["mediumaquamarine"] = "#66CDAAFF",
            ["mediumblue"] = "#0000CDFF",
            ["mediumorchid"] = "#BA55D3FF",
            ["mediumpurple"] = "#9370DBFF",
            ["mediumseagreen"] = "#3CB371FF",
            ["mediumslateblue"] = "#7B68EEFF",
            ["mediumspringgreen"] = "#00FA9AFF",
            ["mediumturquoise"] = "#48D1CCFF",
            ["mediumvioletred"] = "#C71585FF",
            ["midnightblue"] = "#191970FF",
            ["mintcream"] = "#F5FFFAFF",
            ["mistyrose"] = "#FFE4E1FF",
            ["moccasin"] = "#FFE4B5FF",
            ["navajowhite"] = "#FFDEADFF",
            ["oldlace"] = "#FDF5E6FF",
            ["olivedrab"] = "#6B8E23FF",
            ["orangered"] = "#FF4500FF",
            ["orchid"] = "#DA70D6FF",
            ["palegoldenrod"] = "#EEE8AAFF",
            ["palegreen"] = "#98FB98FF",
            ["paleturquoise"] = "#AFEEEEFF",
            ["palevioletred"] = "#DB7093FF",
            ["papayawhip"] = "#FFEFD5FF",
            ["peachpuff"] = "#FFDAB9FF",
            ["peru"] = "#CD853FFF",
            ["pink"] = "#FFC0CBFF",
            ["plum"] = "#DDA0DDFF",
            ["powderblue"] = "#B0E0E6FF",
            ["rosybrown"] = "#BC8F8FFF",
            ["royalblue"] = "#4169E1FF",
            ["saddlebrown"] = "#8B4513FF",
            ["salmon"] = "#FA8072FF",
            ["sandybrown"] = "#F4A460FF",
            ["seagreen"] = "#2E8B57FF",
            ["seashell"] = "#FFF5EEFF",
            ["sienna"] = "#A0522DFF",
            ["skyblue"] = "#87CEEBFF",
            ["slateblue"] = "#6A5ACDFF",
            ["slategray"] = "#708090FF",
            ["slategrey"] = "#708090FF",
            ["snow"] = "#FFFAFAFF",
            ["springgreen"] = "#00FF7FFF",
            ["steelblue"] = "#4682B4FF",
            ["tan"] = "#D2B48CFF",
            ["thistle"] = "#D8BFD8FF",
            ["tomato"] = "#FF6347FF",
            ["turquoise"] = "#40E0D0FF",
            ["violet"] = "#EE82EEFF",
            ["wheat"] = "#F5DEB3FF",
            ["whitesmoke"] = "#F5F5F5FF",
            ["yellowgreen"] = "#9ACD32FF",
        };
        return d;
    }
}
