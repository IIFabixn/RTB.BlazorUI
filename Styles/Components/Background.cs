using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Components;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Background : RTBStyleBase
{
    [Parameter, EditorRequired] public string? Color { get; set; }

    protected override void OnParametersSet()
    {
        if (Color is null) return;
        if (Condition is not null && !Condition.Invoke()) return;

        StyleBuilder.AppendIfNotEmpty("background-color", Color);
    }
}
