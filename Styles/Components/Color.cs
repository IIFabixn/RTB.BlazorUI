using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Color : RTBStyleBase
{
    [Parameter] public string? Value { get; set; }

    protected override StyleBuilder BuildStyle()
    {
        return StyleBuilder.Start.Append("color", Value);
    }
}
