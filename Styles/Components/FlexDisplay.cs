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

    protected override StyleBuilder BuildStyle()
    {
        return StyleBuilder.Start
            .Append("display", "flex")
            .Append("flex-direction", Direction)
            .Append("flex-wrap", Wrap)
            .Append("justify-content", JustifyContent)
            .Append("align-items", AlignItems)
            .Append("align-content", AlignContent)
            .Append("gap", Gap)
            .Append("flex-shrink", Shrink?.ToString())
            .Append("flex-grow", Grow?.ToString());
    }
}
