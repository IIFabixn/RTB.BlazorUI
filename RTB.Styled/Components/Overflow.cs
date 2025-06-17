using Microsoft.AspNetCore.Components;
using RTB.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Styled.Components;

/// <summary>
/// Defaults to 'overflow: auto;' if no parameters are set.
/// </summary>
public class Overflow : RTBStyleBase
{
    public enum OverflowMode { Visible, Hidden, Scroll, Auto }

    [Parameter] public OverflowMode? X { get; set; }
    [Parameter] public OverflowMode? Y { get; set; }
    [Parameter] public OverflowMode? Value { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        if (X == null && Y == null && Value == null)
            return builder.Append("overflow", OverflowMode.Auto.ToCss());
            
        return builder
            .AppendIfNotNull("overflow-x", X?.ToCss())
            .AppendIfNotNull("overflow-y", Y?.ToCss())
            .AppendIfNotNull("overflow", Value?.ToCss());
    }
}
