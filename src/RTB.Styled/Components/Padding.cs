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
        builder.AppendIfNotNull("padding", All);

        builder.AppendIf("padding", $"{Horizontal ?? 0} {Vertical ?? 0}", Horizontal.HasValue || Vertical.HasValue);

        builder.AppendIfNotNull("padding-top", Top);
        builder.AppendIfNotNull("padding-right", Right);
        builder.AppendIfNotNull("padding-bottom", Bottom);
        builder.AppendIfNotNull("padding-left", Left);

        return builder;
    }
}
