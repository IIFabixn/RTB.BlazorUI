using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using System;
using System.Drawing;
using static RTB.Blazor.Styled.Components.Border;

namespace RTB.Blazor.Styled.Components;

public class Border : RTBStyleBase
{
    [Parameter] public SizeUnit Width { get; set; } = 1;
    [Parameter] public RTBColor? Color { get; set; }
    [Parameter] public BorderStyle Style { get; set; } = BorderStyle.Solid;

    [Parameter] public SizeUnit? Radius { get; set; }
    [Parameter] public BorderSide Side { get; set; } = BorderSide.All;
    [Parameter] public BorderCorner Corner { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        builder.BorderRadius(Radius, Corner);

        if (Side == BorderSide.None)
        {
            builder.BorderNone();
        }
        else if (Side == BorderSide.All)
        {
            builder.BorderAll(Width, Color ?? RTBColors.Black, Style);
        }
        else
        {
            if (Side.HasFlag(BorderSide.Top))
                builder.BorderTop(Width, Color ?? RTBColors.Black, Style);
            if (Side.HasFlag(BorderSide.Right))
                builder.BorderRight(Width, Color ?? RTBColors.Black, Style);
            if (Side.HasFlag(BorderSide.Bottom))
                builder.BorderBottom(Width, Color ?? RTBColors.Black, Style);
            if (Side.HasFlag(BorderSide.Left))
                builder.BorderLeft(Width, Color ?? RTBColors.Black, Style);
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

        All = Top | Right | Bottom | Left,

        Horizontal = Left | Right,
        Vertical = Top | Bottom,
    }

    public enum BorderCorner
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 4,
        BottomRight = 8,

        All = TopLeft | TopRight | BottomLeft | BottomRight
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

public static class BorderStyleExtensions
{
    public static StyleBuilder BorderTop(this StyleBuilder builder, SizeUnit width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        builder.Append("border-top", value);

        return builder;
    }

    public static StyleBuilder BorderRight(this StyleBuilder builder, SizeUnit width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        builder.Append("border-right", value);
        return builder;
    }

    public static StyleBuilder BorderBottom(this StyleBuilder builder, SizeUnit width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        builder.Append("border-bottom", value);
        return builder;
    }

    public static StyleBuilder BorderLeft(this StyleBuilder builder, SizeUnit width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        builder.Append("border-left", value);
        return builder;
    }

    public static StyleBuilder BorderAll(this StyleBuilder builder, SizeUnit width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        builder.Append("border", value);
        return builder;
    }

    public static StyleBuilder BorderRadius(this StyleBuilder builder, SizeUnit? radius, BorderCorner corner = BorderCorner.All)
    {
        if (corner == BorderCorner.None)
        {
            return builder.AppendIfNotNull("border-radius", SizeUnit.Zero);
        }

        if (corner == BorderCorner.All)
        {
            return builder.AppendIfNotNull("border-radius", radius);
        }

        if (corner.HasFlag(BorderCorner.TopLeft))
            builder.AppendIfNotNull("border-top-left-radius", radius);
        if (corner.HasFlag(BorderCorner.TopRight))
            builder.AppendIfNotNull("border-top-right-radius", radius);
        if (corner.HasFlag(BorderCorner.BottomLeft))
            builder.AppendIfNotNull("border-bottom-left-radius", radius);
        if (corner.HasFlag(BorderCorner.BottomRight))
            builder.AppendIfNotNull("border-bottom-right-radius", radius);

        return builder;
    }

    public static StyleBuilder BorderNone(this StyleBuilder builder)
    {
        builder.Append("border", "none");
        return builder;
    }
}
