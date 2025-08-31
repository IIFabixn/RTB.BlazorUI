using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using System.Globalization;

namespace RTB.Blazor.Styled.Components
{
    public class Opacity : RTBStyleBase
    {
        [Parameter] public double? Value { get; set; }

        public override IStyleBuilder BuildStyle(IStyleBuilder builder)
        {
            if (!Condition) return builder;

            if (Value is null) return builder;
            var v = Value.Value;
            if (double.IsNaN(v) || double.IsInfinity(v)) return builder;
            v = Math.Clamp(v, 0, 1);

            var s = v.ToString("0.###", CultureInfo.InvariantCulture);
            return builder.Append("opacity", s);
        }
    }

    public static class OpacityExtensions
    {
        public static IStyleBuilder Opacity(this IStyleBuilder builder, double? value)
        {
            if (value is null) return builder;
            var v = Math.Clamp(value.Value, 0, 1);
            var s = v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
            return builder.Append("opacity", s);
        }
    }
}
