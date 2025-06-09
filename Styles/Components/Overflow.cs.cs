using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Components
{
    public class Overflow : RTBStyleBase
    {
        [Parameter] public string? X { get; set; }
        [Parameter] public string? Y { get; set; }
        [Parameter] public string? Value { get; set; }

        protected override void OnParametersSet()
        {
            if (!Condition) return;
            StyleBuilder.AppendIfNotNull("overflow-x", X);
            StyleBuilder.AppendIfNotNull("overflow-y", Y);
            StyleBuilder.AppendIfNotNull("overflow", Value);
            base.OnParametersSet();
        }
    }
}
