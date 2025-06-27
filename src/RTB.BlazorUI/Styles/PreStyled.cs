using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Components;

namespace RTB.Blazor.UI.Styles;

public class PreStyled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.Join(Style?.ToStyle() ?? StyleBuilder.Start);
    }
}
