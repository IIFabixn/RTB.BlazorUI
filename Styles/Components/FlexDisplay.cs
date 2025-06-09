using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Flex : RTBStyleBase
{
    [Parameter] public string? Direction { get; set; }
    [Parameter] public string? Wrap { get; set; }
    [Parameter] public string? JustifyContent { get; set; }
    [Parameter] public string? AlignItems { get; set; }
    [Parameter] public string? AlignContent { get; set; }
    [Parameter] public string? Gap { get; set; }
    [Parameter] public int? Shrink { get; set; }
    [Parameter] public int? Grow { get; set; }

    protected override void OnParametersSet()
    {
        if (!Condition) return;
        
        StyleBuilder.Append("display", "flex");
        StyleBuilder.Append("flex-direction", Direction);
        StyleBuilder.Append("flex-wrap", Wrap);
        StyleBuilder.Append("justify-content", JustifyContent);
        StyleBuilder.Append("align-items", AlignItems);
        StyleBuilder.Append("align-content", AlignContent);
        StyleBuilder.Append("gap", Gap);
        StyleBuilder.Append("flex-shrink", Shrink?.ToString());
        StyleBuilder.Append("flex-grow", Grow?.ToString());
        
        base.OnParametersSet();
    }
}
