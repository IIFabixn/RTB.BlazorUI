using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using System;

namespace RTB.Blazor.Styled.Components;

public class Size : RTBStyleBase
{
    [Parameter] public SizeExpression? Width { get; set; }
    [Parameter] public SizeExpression? Height { get; set; }
    [Parameter] public SizeExpression? MinWidth { get; set; }
    [Parameter] public SizeExpression? MinHeight { get; set; }
    [Parameter] public SizeExpression? MaxWidth { get; set; }
    [Parameter] public SizeExpression? MaxHeight { get; set; }

    [Parameter] public bool FullWidth { get; set; }
    [Parameter] public bool FullHeight { get; set; }

    public override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        if (!Condition) return builder;

        builder.AppendIf("width", "100%", FullWidth);
        builder.Width(Width, MinWidth, MaxWidth);

        builder.AppendIf("height", "100%", FullHeight);
        builder.Height(Height, MinHeight, MaxHeight);

        return builder;
    }
}

public static class SizeExtensions
{
    public static StyleBuilder Height(this StyleBuilder builder, SizeExpression? value, SizeExpression? min = null, SizeExpression? max = null)
    {
        return builder
            .AppendIfNotNull("height", value)
            .AppendIfNotNull("min-height", min)
            .AppendIfNotNull("max-height", max);
    }

    public static StyleBuilder Width(this StyleBuilder builder, SizeExpression? value, SizeExpression? min = null, SizeExpression? max = null)
    {
        return builder
            .AppendIfNotNull("width", value)
            .AppendIfNotNull("min-width", min)
            .AppendIfNotNull("max-width", max);
    }
}
