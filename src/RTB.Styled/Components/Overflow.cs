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

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition) return builder;

        if (X == null && Y == null && Value == null)
            return builder.Append("overflow", OverflowMode.Auto.ToCss());

        return builder.OverflowX(X).OverflowY(Y).Overflow(Value);
    }
}

public static class OverflowExtensions
{
    public static IStyleBuilder Overflow(this IStyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.AppendIfNotNull("overflow", value?.ToCss());
    }
    public static IStyleBuilder OverflowX(this IStyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.AppendIfNotNull("overflow-x", value?.ToCss());
    }
    public static IStyleBuilder OverflowY(this IStyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.AppendIfNotNull("overflow-y", value?.ToCss());
    }
}
