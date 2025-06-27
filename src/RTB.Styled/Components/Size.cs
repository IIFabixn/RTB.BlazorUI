using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using System;

namespace RTB.Blazor.Styled.Components;

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

        builder.AppendIf("width", "100%", FullWidth);
        builder.AppendIfNotNull("width", Width);
        builder.AppendIfNotNull("max-width", MaxWidth);
        builder.AppendIfNotNull("min-width", MinWidth);


        builder.AppendIf("height", "100%", FullHeight);
        builder.AppendIfNotNull("height", Height);
        builder.AppendIfNotNull("max-height", MaxHeight);
        builder.AppendIfNotNull("min-height", MinHeight);

        return builder;
    }
}
