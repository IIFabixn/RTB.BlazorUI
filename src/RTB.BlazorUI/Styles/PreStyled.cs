using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Components;

namespace RTB.Blazor.Styles;

public class PreStyled : RTBStyleBase
{
    [Parameter] public IStyle? Style { get; set; }

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        return builder.Join(Style?.ToStyle() ?? StyleBuilder.Start);
    }
}
