using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Styles.Components;

public class Border : RTBStyleBase
{
    [Parameter] public string Width { get; set; } = "1px";
    [Parameter] public string? Color { get; set; } = "currentColor";
    [Parameter] public BorderStyle Style { get; set; } = BorderStyle.Solid;

    [Parameter] public string? Radius { get; set; } = default!;
    [Parameter] public BorderSide Side { get; set; } = BorderSide.All;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;
        
        if (Side == BorderSide.None)
        {
            StyleBuilder.Append("border", "none");
        }
        else if (Side == BorderSide.All)
        {
            StyleBuilder.Append("border", $"{Width} {Style.ToString().ToLowerInvariant()} {Color}");
            StyleBuilder.Append("border-radius", Radius);
        }
        else
        {
            StyleBuilder.Append("border-radius", Radius);
            StyleBuilder.Append("border-width", Width);
            StyleBuilder.Append("border-style", Style.ToString().ToLowerInvariant());
            StyleBuilder.Append("border-color", Color);

            string value = $"{Width} {Style.ToString().ToLowerInvariant()} {Color}";
            if (Side.HasFlag(BorderSide.Top))
                StyleBuilder.Append("border-top", value);
            if (Side.HasFlag(BorderSide.Right))
                StyleBuilder.Append("border-right", value);
            if (Side.HasFlag(BorderSide.Bottom))
                StyleBuilder.Append("border-bottom", value);
            if (Side.HasFlag(BorderSide.Left))
                StyleBuilder.Append("border-left", value);
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
