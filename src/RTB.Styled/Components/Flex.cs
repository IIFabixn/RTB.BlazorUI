using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RTB.Blazor.Styled.Components;

public class Flex : RTBStyleBase
{
    public enum AxisDirection { Row, RowReverse, Column, ColumnReverse }
    public enum WrapMode { NoWrap, Wrap, WrapReverse }
    public enum Justify { Start, End, Center, SpaceBetween, SpaceAround, SpaceEvenly }
    public enum Align { Start, End, Center, Stretch, Baseline }


    [Parameter] public AxisDirection? Direction { get; set; }
    [Parameter] public WrapMode? Wrap { get; set; }
    [Parameter] public Justify? JustifyContent { get; set; }
    [Parameter] public Align? AlignItems { get; set; }
    [Parameter] public Align? AlignContent { get; set; }

    [Parameter] public Spacing? Gap { get; set; }
    [Parameter] public int? Shrink { get; set; }
    [Parameter] public int? Grow { get; set; }

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition) return builder;

        return builder
            .Append("display", "flex")
            .AppendIfNotNull("flex-direction", Direction?.ToCss())
            .AppendIfNotNull("flex-wrap", Wrap?.ToCss())
            .AppendIfNotNull("justify-content", JustifyContent?.ToCss())
            .AppendIfNotNull("align-items", AlignItems?.ToCss())
            .AppendIfNotNull("align-content", AlignContent?.ToCss())
            .AppendIfNotNull("gap", Gap)
            .AppendIfNotNull("flex-shrink", Shrink?.ToString())
            .AppendIfNotNull("flex-grow", Grow?.ToString());
    }
}

public static class FlexExtensions
{
    public static IStyleBuilder Flex(this IStyleBuilder builder, 
        Flex.AxisDirection? direction = null, 
        Flex.WrapMode? wrap = null,
        Flex.Justify? justifyContent = null,
        Flex.Align? alignItems = null,
        Flex.Align? alignContent = null,
        Spacing? gap = null,
        int? shrink = null,
        int? grow = null)
    {
        return builder
            .Append("display", "flex")
            .AppendIfNotNull("flex-direction", direction?.ToCss())
            .AppendIfNotNull("flex-wrap", wrap?.ToCss())
            .AppendIfNotNull("justify-content", justifyContent?.ToCss())
            .AppendIfNotNull("align-items", alignItems?.ToCss())
            .AppendIfNotNull("align-content", alignContent?.ToCss())
            .AppendIfNotNull("gap", gap)
            .AppendIfNotNull("flex-shrink", shrink?.ToString())
            .AppendIfNotNull("flex-grow", grow?.ToString());
    }
}
