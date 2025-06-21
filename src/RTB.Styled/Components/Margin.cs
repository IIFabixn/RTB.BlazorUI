using System;
using Microsoft.AspNetCore.Components;
using RTB.Styled.Helper;

namespace RTB.Styled.Components;

public class Margin : RTBStyleBase
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
        builder.AppendIfNotNull("margin", All);

        builder.AppendIf("margin", $"{Horizontal ?? 0} {Vertical ?? 0}", Horizontal is not null || Vertical is not null);

        builder.AppendIfNotNull("margin-top", Top);
        builder.AppendIfNotNull("margin-right", Right);
        builder.AppendIfNotNull("margin-bottom", Bottom);
        builder.AppendIfNotNull("margin-left", Left);

        return builder;
    }
}
