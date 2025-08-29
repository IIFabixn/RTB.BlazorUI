using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Color : RTBStyleBase
{
    [Parameter] public RTBColor? Value { get; set; }

    public override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        if (!Condition) return builder;

        return builder.AppendIfNotNull("color", Value);
    }
}

public static class ColorExtensions
{
    public static StyleBuilder Color(this StyleBuilder builder, RTBColor? color)
    {
        return builder.AppendIfNotNull("color", color);
    }
}
