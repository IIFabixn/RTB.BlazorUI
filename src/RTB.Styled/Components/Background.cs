using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Background : RTBStyleBase
{
    [Parameter] public RTBColor? Color { get; set; }

    // TODO: Add support for background image, gradient, etc.

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Background(Color);
    }
}

public static class BackgroundExtensions
{
    public static StyleBuilder Background(this StyleBuilder builder, RTBColor? color)
    {
        return builder.SetIfNotNull("background-color", color);
    }
}
