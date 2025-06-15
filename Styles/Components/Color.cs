using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Color : RTBStyleBase
{
    [Parameter, EditorRequired] public required string Value { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.Append("color", Value);
    }
}
