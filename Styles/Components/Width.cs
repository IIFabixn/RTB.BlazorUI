using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Width : RTBStyleBase
{
    [Parameter] public string Min { get; set; } = string.Empty;
    [Parameter] public string Max { get; set; } = string.Empty;
    [Parameter] public string? Value { get; set; } = null;
    
    [Parameter] public bool Full { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;

        StyleBuilder.AppendIfNotEmpty("min-width", Min)
            .AppendIfNotEmpty("max-width", Max)
            .AppendIf("width", "100%", Full)
            .AppendIfNotEmpty("width", Value);
    }
}
