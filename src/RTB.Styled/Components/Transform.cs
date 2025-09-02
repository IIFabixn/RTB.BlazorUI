using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using RTB.Blazor.Styled.Core;

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

        /// <summary>
        /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildStyle(StyleBuilder builder)
        {
            builder.SetIfNotNull("transform-origin", Origin);

            var parts = Parts?.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            if (parts is { Length: > 0 })
                builder.Set("transform", string.Join(" ", parts));
        }

        /// <summary>
        /// Creates a translate part. E.g. "translate(10px, 20px)".
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string PartTranslate(SizeUnit x, SizeUnit y) => $"translate({x}, {y})";

        /// <summary>
        /// Creates a translateX part. E.g. "translateX(10px)".
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string PartTranslateX(SizeUnit x) => $"translateX({x})";

        /// <summary>
        /// Creates a translateY part. E.g. "translateY(20px)".
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string PartTranslateY(SizeUnit y) => $"translateY({y})";

        /// <summary>
        /// Creates a translateZ part. E.g. "translateZ(30px)".
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static string PartTranslateZ(SizeUnit z) => $"translateZ({z})";

        /// <summary>
        /// Creates a translate3d part. E.g. "translate3d(10px, 20px, 30px)".
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static string PartTranslate3D(SizeUnit x, SizeUnit y, SizeUnit z) => $"translate3d({x}, {y}, {z})";

        /// <summary>
        /// Creates a scale part. E.g. "scale(1.5)" or "scale(1.5, 2)".
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string PartScale(double s) => $"scale({Fmt(s)})";

        /// <summary>
        /// Creates a scale part with separate X and Y factors. E.g. "scale(1.5, 2)".
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <returns></returns>
        public static string PartScale(double sx, double sy) => $"scale({Fmt(sx)}, {Fmt(sy)})";

        /// <summary>
        /// Creates a scaleX part. E.g. "scaleX(1.5)".
        /// </summary>
        /// <param name="sx"></param>
        /// <returns></returns>
        public static string PartScaleX(double sx) => $"scaleX({Fmt(sx)})";

        /// <summary>
        /// Creates a scaleY part. E.g. "scaleY(2)".
        /// </summary>
        /// <param name="sy"></param>
        /// <returns></returns>
        public static string PartScaleY(double sy) => $"scaleY({Fmt(sy)})";

        /// <summary>
        /// Creates a scaleZ part. E.g. "scaleZ(1.2)".
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static string PartScaleZ(double sz) => $"scaleZ({Fmt(sz)})";

        /// <summary>
        /// Creates a scale3d part. E.g. "scale3d(1.5, 2, 1)".
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static string PartScale3D(double sx, double sy, double sz) => $"scale3d({Fmt(sx)}, {Fmt(sy)}, {Fmt(sz)})";

        /// <summary>
        /// Creates a rotate part. E.g. "rotate(30deg)".
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartRotate(double deg) => $"rotate({FmtDeg(deg)})";

        /// <summary>
        /// Creates a rotateX/Y/Z part. E.g. "rotateX(30deg)".
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartRotateX(double deg) => $"rotateX({FmtDeg(deg)})";

        /// <summary>
        /// Creates a rotateY part. E.g. "rotateY(30deg)".
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartRotateY(double deg) => $"rotateY({FmtDeg(deg)})";

        /// <summary>
        /// Creates a rotateZ part. E.g. "rotateZ(30deg)".
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartRotateZ(double deg) => $"rotateZ({FmtDeg(deg)})";

        /// <summary>
        /// Creates a rotate3d part. E.g. "rotate3d(1, 0, 0, 30deg)" to rotate 30 degrees around the X axis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartRotate3D(double x, double y, double z, double deg)
            => $"rotate3d({Fmt(x)}, {Fmt(y)}, {Fmt(z)}, {FmtDeg(deg)})";

        /// <summary>
        /// Creates a skew part. E.g. "skew(10deg, 20deg)".
        /// </summary>
        /// <param name="xDeg"></param>
        /// <param name="yDeg"></param>
        /// <returns></returns>
        public static string PartSkew(double xDeg, double yDeg) => $"skew({FmtDeg(xDeg)}, {FmtDeg(yDeg)})";

        /// <summary>
        /// Creates a skewX part. E.g. "skewX(10deg)".
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartSkewX(double deg) => $"skewX({FmtDeg(deg)})";

        /// <summary>
        /// Creates a skewY part. E.g. "skewY(20deg)".
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static string PartSkewY(double deg) => $"skewY({FmtDeg(deg)})";

        /// <summary>
        /// Creates a perspective part. E.g. "perspective(500px)".
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string PartPerspective(SizeUnit d) => $"perspective({d})";

        /// <summary>
        /// Formats a double with up to 3 decimal places using invariant culture.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static string Fmt(double d) => d.ToString("0.###", CultureInfo.InvariantCulture);

        private static string FmtDeg(double d) => $"{Fmt(d)}deg";
    }

    /// <summary>
    /// Extensions for fluent StyleBuilder usage.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Sets 'transform: none;' to reset any transforms.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static StyleBuilder TransformNone(this StyleBuilder b)
            => b.Set("transform", "none");

        /// <summary>
        /// Sets the 'transform-origin' property.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static StyleBuilder TransformOrigin(this StyleBuilder b, string? origin)
            => b.SetIfNotNull("transform-origin", origin);

        /// <summary>
        /// Sets the 'transform' property by joining the given parts with spaces.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static StyleBuilder Transform(this StyleBuilder b, params string[] parts)
        {
            var cleaned = parts?.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray() ?? [];
            if (cleaned.Length == 0) return b;
            return b.Set("transform", string.Join(" ", cleaned));
        }
    }
}
