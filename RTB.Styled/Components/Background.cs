using System;
using Microsoft.AspNetCore.Components;
using RTB.Styled.Helper;

namespace RTB.Styled.Components;

public class Background : RTBStyleBase
{
    [Parameter] public RTBColor? Color { get; set; }

    // TODO: Add support for background image, gradient, etc.

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.AppendIfNotNull("background-color", Color);
    }
}
