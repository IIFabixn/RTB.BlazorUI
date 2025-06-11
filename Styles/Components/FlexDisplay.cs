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

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("display", "flex")
            .AppendIfNotNull("flex-direction", Direction)
            .AppendIfNotNull("flex-wrap", Wrap)
            .AppendIfNotNull("justify-content", JustifyContent)
            .AppendIfNotNull("align-items", AlignItems)
            .AppendIfNotNull("align-content", AlignContent)
            .AppendIfNotNull("gap", Gap)
            .AppendIfNotNull("flex-shrink", Shrink?.ToString())
            .AppendIfNotNull("flex-grow", Grow?.ToString());
    }
}
