using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Styles.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Components
{
    public class Overflow : RTBStyleBase
    {
        public enum OverflowMode { Visible, Hidden, Scroll, Auto }

        [Parameter] public OverflowMode? X { get; set; }
        [Parameter] public OverflowMode? Y { get; set; }
        [Parameter] public OverflowMode? Value { get; set; }

        protected override StyleBuilder BuildStyle(StyleBuilder builder)
        {
            return builder
                .AppendIfNotNull("overflow-x", X?.ToCss())
                .AppendIfNotNull("overflow-y", Y?.ToCss())
                .AppendIfNotNull("overflow", Value?.ToCss());
        }
    }
}
