using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Styles.Helper;

namespace RTB.BlazorUI.Styles.Components;

public class Flex : RTBStyleBase
{
    public enum AxisDirection { Row, RowReverse, Column, ColumnReverse }
    public enum WrapMode { NoWrap, Wrap, WrapReverse }
    public enum Justify { Start, End, Center, Between, Around, Evenly }
    public enum Align { Start, End, Center, Stretch, Baseline }


    [Parameter] public AxisDirection? Direction { get; set; }
    [Parameter] public WrapMode? Wrap { get; set; }
    [Parameter] public Justify? JustifyContent { get; set; }
    [Parameter] public Align? AlignItems { get; set; }
    [Parameter] public Align? AlignContent { get; set; }

    [Parameter] public Spacing? Gap { get; set; }
    [Parameter] public int? Shrink { get; set; }
    [Parameter] public int? Grow { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
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
