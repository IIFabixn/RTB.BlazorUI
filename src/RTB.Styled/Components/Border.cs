using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Border : RTBStyleBase
{
    [Parameter] public SizeUnit Width { get; set; } = 1;
    [Parameter] public RTBColor? Color { get; set; }
    [Parameter] public BorderStyle Style { get; set; } = BorderStyle.Solid;

    [Parameter] public SizeUnit? Radius { get; set; }
    [Parameter] public BorderSide Side { get; set; } = BorderSide.All;

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        builder.AppendIfNotNull("border-radius", Radius);

        if (Side == BorderSide.None)
        {
            builder.Append("border", "none");
        }
        else if (Side == BorderSide.All)
        {
            builder.AppendIf("border", $"{Width} {Style.ToCss()} {Color}", condition: Color is not null);
        }
        else
        {
            string value = $"{Width} {Style.ToCss()} {Color}";
            if (Side.HasFlag(BorderSide.Top))
                builder.Append("border-top", value);
            if (Side.HasFlag(BorderSide.Right))
                builder.Append("border-right", value);
            if (Side.HasFlag(BorderSide.Bottom))
                builder.Append("border-bottom", value);
            if (Side.HasFlag(BorderSide.Left))
                builder.Append("border-left", value);
        }

        return builder;
    }

    public enum BorderSide
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
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
