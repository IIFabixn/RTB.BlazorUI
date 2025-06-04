using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Other : RTBStyleBase
{
    [Parameter] public string Property { get; set; } = string.Empty;
    [Parameter] public string? Value { get; set; }
    [Parameter] public string? Raw { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;

        StyleBuilder.AppendIfNotEmpty(Property, Value)
            .AppendStyle(Raw);
    }
}
