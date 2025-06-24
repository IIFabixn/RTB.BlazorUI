using System;
using Microsoft.AspNetCore.Components;
using RTB.Styled;
using RTB.Styled.Components;

namespace RTB.BlazorUI.Styles;

public class PreStyled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.Join(Style?.ToStyle() ?? StyleBuilder.Start);
    }
}
