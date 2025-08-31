using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components
{
    public class Visibility : RTBStyleBase
    {
        public enum Mode { Visible, Hidden, Collapse }

        [Parameter] public Mode Value { get; set; } = Mode.Visible;
        
        public override IStyleBuilder BuildStyle(IStyleBuilder builder)
        {
            if (!Condition) return builder;

            return builder.Visibility(Value);
        }
    }

    public static class VisibilityExtensions
    {
        public static IStyleBuilder Visibility(this IStyleBuilder builder, Visibility.Mode value)
        {
            return builder.Append("visibility", value.ToCss());
        }
    }
}
