using Microsoft.AspNetCore.Components;
using RTB.Styled.Helper;
using System;

namespace RTB.Styled.Components;

public class Size : RTBStyleBase
{
    [Parameter] public SizeUnit? Width { get; set; }
    [Parameter] public SizeUnit? Height { get; set; }
    [Parameter] public SizeUnit? MinWidth { get; set; }
    [Parameter] public SizeUnit? MinHeight { get; set; }
    [Parameter] public SizeUnit? MaxWidth { get; set; }
    [Parameter] public SizeUnit? MaxHeight { get; set; }

    [Parameter] public bool FullWidth { get; set; }
    [Parameter] public bool FullHeight { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        if (Width is not null)
            builder.Width((SizeUnit)Width);

        if (Height is not null)
            builder.Height((SizeUnit)Height);

        if (FullHeight)
            builder.FullHeight();

        if (FullWidth)
            builder.FullWidth();

        if (MinHeight is not null)
            builder.MinWidth((SizeUnit)MinHeight);

        if (MaxHeight is not null)
            builder.MaxWidth((SizeUnit)MaxHeight);

        if (MinWidth is not null)
            builder.MinWidth((SizeUnit)MinWidth);

        if (MaxWidth is not null)
            builder.MaxWidth((SizeUnit)MaxWidth);

        return builder;
    }
}
