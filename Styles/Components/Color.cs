using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Color : RTBStyleBase
{
    [Parameter] public string? Value { get; set; }

    protected override void OnParametersSet()
    {
        if (!Condition) return;
        
        StyleBuilder.AppendIfNotEmpty("color", Value);

        base.OnParametersSet();
    }
}
