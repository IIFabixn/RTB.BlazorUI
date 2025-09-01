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

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder
            .Set("display", "flex")
            .SetIfNotNull("flex-direction", Direction?.ToCss())
            .SetIfNotNull("flex-wrap", Wrap?.ToCss())
            .SetIfNotNull("justify-content", JustifyContent?.ToCss())
            .SetIfNotNull("align-items", AlignItems?.ToCss())
            .SetIfNotNull("align-content", AlignContent?.ToCss())
            .SetIfNotNull("gap", Gap)
            .SetIfNotNull("flex-shrink", Shrink?.ToString())
            .SetIfNotNull("flex-grow", Grow?.ToString());
    }
}

public static class FlexExtensions
{
    public static StyleBuilder Flex(this StyleBuilder builder, 
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
            .Set("display", "flex")
            .SetIfNotNull("flex-direction", direction?.ToCss())
            .SetIfNotNull("flex-wrap", wrap?.ToCss())
            .SetIfNotNull("justify-content", justifyContent?.ToCss())
            .SetIfNotNull("align-items", alignItems?.ToCss())
            .SetIfNotNull("align-content", alignContent?.ToCss())
            .SetIfNotNull("gap", gap)
            .SetIfNotNull("flex-shrink", shrink?.ToString())
            .SetIfNotNull("flex-grow", grow?.ToString());
    }
}
