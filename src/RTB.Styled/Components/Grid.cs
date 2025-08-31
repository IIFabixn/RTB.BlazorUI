using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Grid : RTBStyleBase
{
    public enum Placement 
    { 
        Normal,
        Start,
        End,
        Center,
        Stretch,
        FlexEnd, 
        FlexStart
    }

    [Parameter] public string TemplateColumns { get; set; } = "1fr";
    [Parameter] public string TemplateRows { get; set; } = "1fr";
    [Parameter] public Spacing? Gap { get; set; }
    [Parameter] public Placement? ItemPlacement { get; set; }

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition) return builder;

        return builder
            .Append("display", "grid")
            .Append("grid-template-columns", TemplateColumns)
            .Append("grid-template-rows", TemplateRows)
            .AppendIfNotNull("gap", Gap)
            .AppendIfNotNull("place-items", ItemPlacement?.ToCss());
    }
}

public static class GridExtensions
{
    public static IStyleBuilder Grid(this IStyleBuilder builder, 
        string templateColumns = "1fr", 
        string templateRows = "1fr", 
        Spacing? gap = null, 
        Grid.Placement? itemPlacement = null)
    {
        return builder
            .Append("display", "grid")
            .Append("grid-template-columns", templateColumns)
            .Append("grid-template-rows", templateRows)
            .AppendIfNotNull("gap", gap)
            .AppendIfNotNull("place-items", itemPlacement?.ToCss());
    }
}
