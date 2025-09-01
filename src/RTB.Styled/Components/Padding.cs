using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Padding : RTBStyleBase
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
        builder.PaddingAll(All);

        builder.SetIf("padding", $"{Vertical ?? 0} {Horizontal ?? 0}", Horizontal.HasValue || Vertical.HasValue);

        builder.PaddingTop(Top);
        builder.PaddingRight(Right);
        builder.PaddingBottom(Bottom);
        builder.PaddingLeft(Left);
    }
}

public static class PaddingExtensions
{
    public static StyleBuilder PaddingAll(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding", value);
    }

    public static StyleBuilder PaddingTop(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-top", value);
    }

    public static StyleBuilder PaddingRight(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-right", value);
    }

    public static StyleBuilder PaddingBottom(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-bottom", value);
    }

    public static StyleBuilder PaddingLeft(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-left", value);
    }
}
