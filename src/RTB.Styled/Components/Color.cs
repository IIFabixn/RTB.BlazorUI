using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Color : RTBStyleBase
{
    [Parameter] public RTBColor? Value { get; set; }

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Color(Value);
    }
}

public static class ColorExtensions
{
    public static StyleBuilder Color(this StyleBuilder builder, RTBColor? color)
    {
        return builder.SetIfNotNull("color", color);
    }
}
