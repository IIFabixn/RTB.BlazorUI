using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Height : RTBStyleBase
{
    [Parameter] public string Min { get; set; } = string.Empty;
    [Parameter] public string Max { get; set; } = string.Empty;
    [Parameter] public string? Value { get; set; } = null;

    [Parameter] public bool Full { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;

        StyleBuilder.AppendIfNotEmpty("min-height", Min)
            .AppendIfNotEmpty("max-height", Max)
            .AppendIf("height", "100%", Full)
            .AppendIfNotEmpty("height", Value);
    }
}
