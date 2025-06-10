using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Transition : RTBStyleBase
{
    [Parameter] public string Delay { get; set; } = "0s";
    [Parameter] public string Duration { get; set; } = "0s";
    [Parameter] public string Property { get; set; } = "all";
    [Parameter] public string TimingFunction { get; set; } = "ease";
    [Parameter] public string? Behavior { get; set; } = "normal";

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("transition-delay", Delay)
            .Append("transition-duration", Duration)
            .Append("transition-property", Property)
            .Append("transition-timing-function", TimingFunction)
            .Append("transition-behavior", Behavior);
    }
}
