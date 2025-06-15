using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Components;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Background : RTBStyleBase
{
    [Parameter] public string? Color { get; set; }

    // TODO: Add support for background image, gradient, etc.

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.AppendIfNotNull("background-color", Color);
    }
}
