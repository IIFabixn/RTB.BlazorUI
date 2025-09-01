using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
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

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.SetIf("width", "100%", FullWidth);
        builder.Width(Width, MinWidth, MaxWidth);

        builder.SetIf("height", "100%", FullHeight);
        builder.Height(Height, MinHeight, MaxHeight);
    }
}

public static class SizeExtensions
{
    public static StyleBuilder Height(this StyleBuilder builder, SizeExpression? value = null, SizeExpression? min = null, SizeExpression? max = null)
    {
        return builder
            .SetIfNotNull("height", value)
            .SetIfNotNull("min-height", min)
            .SetIfNotNull("max-height", max);
    }

    public static StyleBuilder Width(this StyleBuilder builder, SizeExpression? value = null, SizeExpression? min = null, SizeExpression? max = null)
    {
        return builder
            .SetIfNotNull("width", value)
            .SetIfNotNull("min-width", min)
            .SetIfNotNull("max-width", max);
    }
}
