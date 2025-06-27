using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Color : RTBStyleBase
{
    [Parameter] public RTBColor? Value { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.AppendIfNotNull("color", Value);
    }
}
