using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Modern transform component: compose transforms via Parts and set optional Origin.
    /// </summary>
    public class Transform : RTBStyleBase
    {
        /// <summary>
        /// List of transform parts like "translateY(8px)", "rotate(3deg)". Joined by spaces.
        /// </summary>
        [Parameter] public IEnumerable<string>? Parts { get; set; }

        /// <summary>
        /// Optional "transform-origin": e.g. "center", "left top", "20px 50%".
        /// </summary>
        [Parameter] public string? Origin { get; set; }

        public override StyleBuilder BuildStyle(StyleBuilder builder)
        {
            builder.AppendIfNotNull("transform-origin", Origin);

            var parts = Parts?.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            if (parts is { Length: > 0 })
                return builder.Append("transform", string.Join(" ", parts));

            // no transform when empty
            return builder;
        }
    }

    public static class TransformExtensions
    {
        public static StyleBuilder TransformNone(this StyleBuilder b)
            => b.Append("transform", "none");

        public static StyleBuilder TransformOrigin(this StyleBuilder b, string? origin)
            => b.AppendIfNotNull("transform-origin", origin);

        public static StyleBuilder Transform(this StyleBuilder b, params string[] parts)
        {
            var cleaned = parts?.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray() ?? Array.Empty<string>();
            if (cleaned.Length == 0) return b;
            return b.Append("transform", string.Join(" ", cleaned));
        }

        // Helpers to create parts:
        public static string PartTranslate(SizeUnit x, SizeUnit y) => $"translate({x}, {y})";
        public static string PartTranslateX(SizeUnit x) => $"translateX({x})";
        public static string PartTranslateY(SizeUnit y) => $"translateY({y})";
        public static string PartTranslateZ(SizeUnit z) => $"translateZ({z})";
        public static string PartTranslate3D(SizeUnit x, SizeUnit y, SizeUnit z) => $"translate3d({x}, {y}, {z})";

        public static string PartScale(double s) => $"scale({Fmt(s)})";
        public static string PartScale(double sx, double sy) => $"scale({Fmt(sx)}, {Fmt(sy)})";
        public static string PartScaleX(double sx) => $"scaleX({Fmt(sx)})";
        public static string PartScaleY(double sy) => $"scaleY({Fmt(sy)})";
        public static string PartScaleZ(double sz) => $"scaleZ({Fmt(sz)})";
        public static string PartScale3D(double sx, double sy, double sz) => $"scale3d({Fmt(sx)}, {Fmt(sy)}, {Fmt(sz)})";

        public static string PartRotate(double deg) => $"rotate({FmtDeg(deg)})";
        public static string PartRotateX(double deg) => $"rotateX({FmtDeg(deg)})";
        public static string PartRotateY(double deg) => $"rotateY({FmtDeg(deg)})";
        public static string PartRotateZ(double deg) => $"rotateZ({FmtDeg(deg)})";
        public static string PartRotate3D(double x, double y, double z, double deg)
            => $"rotate3d({Fmt(x)}, {Fmt(y)}, {Fmt(z)}, {FmtDeg(deg)})";

        public static string PartSkew(double xDeg, double yDeg) => $"skew({FmtDeg(xDeg)}, {FmtDeg(yDeg)})";
        public static string PartSkewX(double deg) => $"skewX({FmtDeg(deg)})";
        public static string PartSkewY(double deg) => $"skewY({FmtDeg(deg)})";

        public static string PartPerspective(SizeUnit d) => $"perspective({d})";

        private static string Fmt(double d) => d.ToString("0.###", CultureInfo.InvariantCulture);
        private static string FmtDeg(double d) => $"{Fmt(d)}deg";
    }
}
