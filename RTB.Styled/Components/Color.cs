using System;
using Microsoft.AspNetCore.Components;
using RTB.Styled.Helper;

namespace RTB.Styled.Components;

public class Color : RTBStyleBase
{
    [Parameter] public RTBColor? Value { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.AppendIfNotNull("color", Value);
    }
}
