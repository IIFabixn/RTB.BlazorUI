using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class PreStyled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    protected override void OnParametersSet()
    {
        if (Style is null) return;
        if (!Condition) return;

        StyleBuilder.AppendStyle(Style);
        
        base.OnParametersSet();
    }
}
