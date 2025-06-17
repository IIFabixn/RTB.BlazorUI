using System;
using Microsoft.AspNetCore.Components;
using RTB.Styled.Helper;

namespace RTB.Styled.Components;

public class Padding : RTBStyleBase
{
    [Parameter] public Spacing? All { get; set; }
    [Parameter] public Spacing? Top { get; set; }
    [Parameter] public Spacing? Right { get; set; }
    [Parameter] public Spacing? Bottom { get; set; }
    [Parameter] public Spacing? Left { get; set; }
    [Parameter] public Spacing? Horizontal { get; set; }
    [Parameter] public Spacing? Vertical { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        if (!string.IsNullOrEmpty(All))
            builder.Append("padding", All);

        if (!string.IsNullOrEmpty(Horizontal) || !string.IsNullOrEmpty(Vertical))
            builder.Append("padding", $"{Vertical ?? "0"} {Horizontal ?? "0"}");

        return builder
            .AppendIfNotNull("padding-top", Top)
            .AppendIfNotNull("padding-right", Right)
            .AppendIfNotNull("padding-bottom", Bottom)
            .AppendIfNotNull("padding-left", Left);
    }
}
