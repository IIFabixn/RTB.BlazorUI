using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Color : RTBStyleBase
{
    [Parameter] public RTBColor? Value { get; set; }

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition) return builder;

        return builder.AppendIfNotNull("color", Value);
    }
}

public static class ColorExtensions
{
    public static IStyleBuilder Color(this IStyleBuilder builder, RTBColor? color)
    {
        return builder.AppendIfNotNull("color", color);
    }
}
