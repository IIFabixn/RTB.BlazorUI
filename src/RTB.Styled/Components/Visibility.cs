using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
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
        
        protected override void BuildStyle(StyleBuilder builder)
        {
            builder.Visibility(Value);
        }
    }

    public static class VisibilityExtensions
    {
        public static StyleBuilder Visibility(this StyleBuilder builder, Visibility.Mode value)
        {
            return builder.Set("visibility", value.ToCss());
        }
    }
}
