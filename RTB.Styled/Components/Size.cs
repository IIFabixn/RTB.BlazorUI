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
        return builder
            .AppendIfNotNull("width", Width)
            .AppendIf("width", "100%", FullWidth)
            .AppendIfNotNull("height", Height)
            .AppendIf("height", "100%", FullHeight)
            .AppendIfNotNull("min-width", MinWidth)
            .AppendIfNotNull("min-height", MinHeight)
            .AppendIfNotNull("max-width", MaxWidth)
            .AppendIfNotNull("max-height", MaxHeight);
    }
}
