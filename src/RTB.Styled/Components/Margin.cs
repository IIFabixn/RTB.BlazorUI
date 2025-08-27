using System;
using Microsoft.AspNetCore.Components;
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

    public override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        builder.AppendIfNotNull("margin", All);

        builder.AppendIf("margin", $"{Vertical ?? 0} {Horizontal ?? 0}", Horizontal is not null || Vertical is not null);

        builder.AppendIfNotNull("margin-top", Top);
        builder.AppendIfNotNull("margin-right", Right);
        builder.AppendIfNotNull("margin-bottom", Bottom);
        builder.AppendIfNotNull("margin-left", Left);

        return builder;
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
            .AppendIfNotNull("margin", all)
            .AppendIf("margin", $"{vertical ?? 0} {horizontal ?? 0}", horizontal is not null || vertical is not null)
            .AppendIfNotNull("margin-top", top)
            .AppendIfNotNull("margin-right", right)
            .AppendIfNotNull("margin-bottom", bottom)
            .AppendIfNotNull("margin-left", left);
    }
}
