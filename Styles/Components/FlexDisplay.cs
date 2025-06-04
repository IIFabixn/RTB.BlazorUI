using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class FlexDisplay : RTBStyleBase
{
    [Parameter] public string? Direction { get; set; }
    [Parameter] public string? Wrap { get; set; }
    [Parameter] public string? JustifyContent { get; set; }
    [Parameter] public string? AlignItems { get; set; }
    [Parameter] public string? AlignContent { get; set; }
    [Parameter] public string? Gap { get; set; }

    protected override void OnParametersSet()
    {
        if (!Condition) return;
        
        StyleBuilder.Append("display", "flex");
        StyleBuilder.AppendIfNotEmpty("flex-direction", Direction);
        StyleBuilder.AppendIfNotEmpty("flex-wrap", Wrap);
        StyleBuilder.AppendIfNotEmpty("justify-content", JustifyContent);
        StyleBuilder.AppendIfNotEmpty("align-items", AlignItems);
        StyleBuilder.AppendIfNotEmpty("align-content", AlignContent);
        StyleBuilder.AppendIfNotEmpty("gap", Gap);
        
        base.OnParametersSet();
    }
}
