using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styles;

public class PreStyled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    protected override void BuildStyle(StyleBuilder builder)
    {
        return;
    }
}
