using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class PreStyled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.AppendStyle(Style);
    }
}
