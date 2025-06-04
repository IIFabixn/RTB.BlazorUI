using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Styled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Style is null) return;
        if (!Condition) return;

        this.StyleBuilder.Join(Style.ToStyle(StyleBuilder));
    }
}
