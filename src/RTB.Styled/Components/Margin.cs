using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Margin : RTBStyleBase
{
    [Parameter] public Spacing? All { get; set; }
    [Parameter] public Spacing? Top { get; set; }
    [Parameter] public Spacing? Right { get; set; }
    [Parameter] public Spacing? Bottom { get; set; }
    [Parameter] public Spacing? Left { get; set; }
    [Parameter] public Spacing? Horizontal { get; set; }
    [Parameter] public Spacing? Vertical { get; set; }

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.SetIfNotNull("margin", All);

        builder.SetIf("margin", $"{Vertical ?? 0} {Horizontal ?? 0}", Horizontal is not null || Vertical is not null);

        builder.SetIfNotNull("margin-top", Top);
        builder.SetIfNotNull("margin-right", Right);
        builder.SetIfNotNull("margin-bottom", Bottom);
        builder.SetIfNotNull("margin-left", Left);
    }
}

public static class MarginExtensions
{
    public static StyleBuilder Margin(this StyleBuilder builder,
        Spacing? all = null,
        Spacing? top = null,
        Spacing? right = null,
        Spacing? bottom = null,
        Spacing? left = null,
        Spacing? horizontal = null,
        Spacing? vertical = null)
    {
        return builder
            .SetIfNotNull("margin", all)
            .SetIf("margin", $"{vertical ?? 0} {horizontal ?? 0}", horizontal is not null || vertical is not null)
            .SetIfNotNull("margin-top", top)
            .SetIfNotNull("margin-right", right)
            .SetIfNotNull("margin-bottom", bottom)
            .SetIfNotNull("margin-left", left);
    }
}
