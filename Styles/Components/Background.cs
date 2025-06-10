using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Components;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Background : RTBStyleBase
{
    [Parameter] public string? Color { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.Append("background-color", Color);
    }
}
