using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RTB.BlazorUI.Styles.Helper
{
    /// <summary>
    /// Represents a color in the RGBA (Red, Green, Blue, Alpha) color space.
    /// </summary>
    /// <remarks>This type provides methods for creating and manipulating colors, including conversion from
    /// hexadecimal strings, blending, and adjustments to lightness, saturation, and alpha transparency. It supports
    /// implicit conversions to and from hexadecimal color strings.</remarks>
    /// <param name="R"></param>
    /// <param name="G"></param>
    /// <param name="B"></param>
    /// <param name="A"></param>
    [TypeConverter(typeof(RTBColorConverter))]
    public readonly record struct RTBColor(byte R, byte G, byte B, byte A)
    {
        public static RTBColor FromRgb(byte r, byte g, byte b) => new(r, g, b, 255);

        public static RTBColor FromRgba(byte r, byte g, byte b, byte a) => new(r, g, b, a);

        public static RTBColor Parse(string hex) => HexToColor(hex);

        public byte Alpha => A;

        public byte Red => R;

        public byte Green => G;

        public byte Blue => B;

        public string HexRgb => $"#{R:X2}{G:X2}{B:X2}";

        public string HexRgba => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

        public override string ToString() => HexRgba;

        public static implicit operator RTBColor(string hex) => Parse(hex);

        public static implicit operator string(RTBColor c) => c.HexRgba;

        public RTBColor WithAlpha(double alpha) => new(R, G, B, (byte)(Clamp(alpha) * 255));

        public RTBColor Lighten(double pct) => HslShift(L: pct);

        public RTBColor Darken(double pct) => HslShift(L: -pct);

        public RTBColor Saturate(double pct) => HslShift(S: pct);

        public RTBColor Desaturate(double pct) => HslShift(S: -pct);

        public RTBColor Blend(RTBColor target, double pct)
        {
            pct = Clamp(pct);
            byte lerp(byte a, byte b) => (byte)(a + (b - a) * pct);
            return new(
                lerp(R, target.R),
                lerp(G, target.G),
                lerp(B, target.B),
                lerp(A, target.A));
        }

        private static double Clamp(double v) => v < 0 ? 0 : v > 1 ? 1 : v;

        private RTBColor HslShift(double H = 0, double S = 0, double L = 0)
        {
            // convert to HSL, shift, convert back (weights per CSS spec)
            ColorToHsl(this, out var h, out var s, out var l);
            h = (h + H) % 1;
            s = Clamp(s + S);
            l = Clamp(l + L);
            return HslToColor(h, s, l, A);
        }

        /// <exception cref="FormatException"></exception>
        private static RTBColor HexToColor(string hex)
        {
            if (hex.StartsWith('#')) hex = hex[1..];

            return hex.Length switch
            {
                3 => new((byte)(Convert.ToByte(hex[0].ToString(), 16) * 17),
                         (byte)(Convert.ToByte(hex[1].ToString(), 16) * 17),
                         (byte)(Convert.ToByte(hex[2].ToString(), 16) * 17),
                         byte.MaxValue),

                6 => new(Convert.ToByte(hex[..2], 16),
                         Convert.ToByte(hex[2..4], 16),
                         Convert.ToByte(hex[4..6], 16),
                         byte.MaxValue),

                8 => new(Convert.ToByte(hex[..2], 16),
                         Convert.ToByte(hex[2..4], 16),
                         Convert.ToByte(hex[4..6], 16),
                         Convert.ToByte(hex[6..8], 16)),

                _ => throw new FormatException($"Unrecognized color format '{hex}'.")
            };
        }

        private static void ColorToHsl(RTBColor color, out double hue, out double satuation, out double luminance)
        {
            double r = color.R / 255.0, g = color.G / 255.0, b = color.B / 255.0;
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            luminance = (max + min) / 2;

            if (Math.Abs(max - min) < 0.001) { hue = satuation = 0; return; }

            double d = max - min;
            satuation = luminance > .5 ? d / (2 - max - min) : d / (max + min);

            hue = max switch
            {
                var _ when max == r => (g - b) / d + (g < b ? 6 : 0),
                var _ when max == g => (b - r) / d + 2,
                _ => (r - g) / d + 4
            };
            hue /= 6;
        }

        private static RTBColor HslToColor(double hue, double satuation, double luminance, byte alpha)
        {
            if (Math.Abs(satuation) < 0.001)
            {
                byte v = (byte)(luminance * 255);
                return new(alpha, v, v, v);
            }

            double q = luminance < .5 ? luminance * (1 + satuation) : luminance + satuation - luminance * satuation;
            double p = 2 * luminance - q;
            double ToRgb(double t)
            {
                if (t < 0) t += 1; if (t > 1) t -= 1;
                if (t < 1.0 / 6) return p + (q - p) * 6 * t;
                if (t < 1.0 / 2) return q;
                if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
                return p;
            }

            byte r = (byte)(ToRgb(hue + 1.0 / 3) * 255);
            byte g = (byte)(ToRgb(hue) * 255);
            byte b = (byte)(ToRgb(hue - 1.0 / 3) * 255);
            return new(alpha, r, g, b);
        }
    }

    internal sealed class RTBColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? _, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(_, sourceType);

        public override object? ConvertFrom(ITypeDescriptorContext? _, CultureInfo? __, object value)
        {
            if (value is string s) return RTBColor.Parse(s);
            return base.ConvertFrom(_, __, value);
        }
    }

    /// <summary>
    /// Provides commonly used colors in the RTB Blazor UI framework.
    /// </summary>
    public static class RTBColors
    {
        public static RTBColor White => "#FFFFFFFF";
        public static RTBColor Black => "#FF000000";

        public static RTBColor Yellow => "#FFFFFF00";
        public static RTBColor Red => "#FFFF0000";
        public static RTBColor Blue => "#FF0000FF";
        public static RTBColor Orange => "#FFFF8800";
        public static RTBColor Green => "#FF00FF00";
        public static RTBColor Magenta => "#FFFF00FF";

        public static RTBColor Transparent => "#00000000";
    }
}