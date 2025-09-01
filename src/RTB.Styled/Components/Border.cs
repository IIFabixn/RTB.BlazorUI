using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Drawing;
using static RTB.Blazor.Styled.Components.Border;

namespace RTB.Blazor.Styled.Components;

public class Border : RTBStyleBase
{
    [Parameter] public BorderSide? Side { get; set; }
    [Parameter] public BorderStyle Style { get; set; } = BorderStyle.Solid;
    [Parameter] public SizeUnit? Width { get; set; }
    [Parameter] public RTBColor? Color { get; set; }

    [Parameter] public SizeUnit? Radius { get; set; }
    [Parameter] public BorderCorner? Corner { get; set; }

    protected override void BuildStyle(StyleBuilder builder)
    {
        if (Corner is not null) builder.BorderRadius(Radius, Corner);
        if (Side is not null) builder.Border(Width, Style, Color, Side);
    }

    [Flags]
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

    [Flags]
    public enum BorderCorner
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 4,
        BottomRight = 8,

        Top = TopLeft | TopRight,
        Right = TopRight | BottomRight,
        Bottom = BottomRight | BottomLeft,
        Left = BottomLeft | TopRight,

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
    public static StyleBuilder BorderAll(this StyleBuilder builder, SizeUnit? width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        return builder.Set("border", value);
    }

    public static StyleBuilder BorderNone(this StyleBuilder builder)
    {
        return builder.Set("border", "unset");
    }

    public static StyleBuilder BorderRadiusAll(this StyleBuilder builder, SizeUnit? radius)
    {
        if (radius is null) return builder;
        return builder.Set("border-radius", radius);
    }

    public static StyleBuilder BorderRadiusNone(this StyleBuilder builder)
    {
        return builder.Set("border-radius", "unset");
    }

    public static StyleBuilder Border(this StyleBuilder builder, SizeUnit? width, BorderStyle style, RTBColor? color, BorderSide? side)
    {
        if (side is null) return builder;

        width ??= SizeUnit.Px(1);
        color ??= RTBColors.Black;

        var s = (BorderSide)side;
        var c = (RTBColor)color;

        if (side == BorderSide.None) return builder.BorderNone();
        if (side == BorderSide.All) return builder.BorderAll(width, c, style);

        string value = $"{width} {style.ToCss()} {color}";
        if (s.HasFlag(BorderSide.Top))
            builder.Set("border-top", value);
        if (s.HasFlag(BorderSide.Right))
            builder.Set("border-right", value);
        if (s.HasFlag(BorderSide.Bottom))
            builder.Set("border-bottom", value);
        if (s.HasFlag(BorderSide.Left))
            builder.Set("border-left", value);

        return builder;
    }

    public static StyleBuilder BorderRadius(this StyleBuilder builder, SizeUnit? radius, BorderCorner? corner)
    {
        if (corner is null) return builder;

        if (corner == BorderCorner.None) return builder.BorderRadiusNone();

        radius ??= SizeUnit.Px(1);
        if (corner == BorderCorner.All) return builder.BorderRadiusAll(radius);

        var c = (BorderCorner)corner;
        if (c.HasFlag(BorderCorner.TopLeft))
            builder.Set("border-top-left-radius", radius);
        if (c.HasFlag(BorderCorner.TopRight))
            builder.Set("border-top-right-radius", radius);
        if (c.HasFlag(BorderCorner.BottomLeft))
            builder.Set("border-bottom-left-radius", radius);
        if (c.HasFlag(BorderCorner.BottomRight))
            builder.Set("border-bottom-right-radius", radius);

        return builder;
    }
}
