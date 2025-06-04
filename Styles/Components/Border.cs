using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Border : RTBStyleBase
{
    [Parameter] public string Width { get; set; } = "1px";
    [Parameter] public string? Color { get; set; } = default!;
    [Parameter] public BorderStyle Style { get; set; } = BorderStyle.Solid;

    [Parameter] public string? Radius { get; set; } = default!;
    [Parameter] public BorderSide Side { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Condition is not null && !Condition.Invoke()) return;
        
        if (Side == BorderSide.None)
        {
            StyleBuilder.AppendIfNotEmpty("border", "none");
        }
        else
        {
            StyleBuilder.AppendIfNotEmpty("border-width", Width);
            StyleBuilder.AppendIfNotEmpty("border-style", Style.ToString().ToLowerInvariant());
            StyleBuilder.AppendIfNotEmpty("border-color", Color);
            if (Radius is not null)
            {
                StyleBuilder.AppendIfNotEmpty("border-radius", Radius);
            }

            if (Side != BorderSide.All)
            {
                StyleBuilder.AppendIfNotEmpty("border-" + Side.ToString().ToLowerInvariant(), Width + " " + Style.ToString().ToLowerInvariant() + " " + Color);
            }
        }
    }

    public enum BorderSide
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
        Left = 4,
        Horizontal = Left | Right,
        Vertical = Top | Bottom,
        All = Top | Right | Bottom | Left
    }

    public enum BorderStyle
    {
        None,
        Solid,
        Dotted,
        Dashed,
        Double,
        Groove,
        Ridge,
        Inset,
        Outset
    }
}
