using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Defaults to 'overflow: auto;' if no parameters are set.
/// </summary>
public class Overflow : RTBStyleBase
{
    public enum OverflowMode { Visible, Hidden, Scroll, Auto }

    [Parameter] public OverflowMode? X { get; set; }
    [Parameter] public OverflowMode? Y { get; set; }
    [Parameter] public OverflowMode? Value { get; set; }

    protected override void BuildStyle(StyleBuilder builder)
    {
        if (X == null && Y == null && Value == null)
        {
            builder.Set("overflow", OverflowMode.Auto.ToCss());
            return;
        }

        builder.OverflowX(X).OverflowY(Y).Overflow(Value);
    }
}

public static class OverflowExtensions
{
    public static StyleBuilder Overflow(this StyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.Other("overflow", value?.ToCss());
    }
    public static StyleBuilder OverflowX(this StyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.Other("overflow-x", value?.ToCss());
    }
    public static StyleBuilder OverflowY(this StyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.Other("overflow-y", value?.ToCss());
    }
}
