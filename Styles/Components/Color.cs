using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Color : RTBStyleBase
{
    [Parameter] public string? Value { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Condition is not null && !Condition.Invoke()) return;
        
        StyleBuilder.AppendIfNotEmpty("color", Value);
    }
}
