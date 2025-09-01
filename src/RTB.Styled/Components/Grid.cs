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

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder
            .Set("display", "grid")
            .Set("grid-template-columns", TemplateColumns)
            .Set("grid-template-rows", TemplateRows)
            .SetIfNotNull("gap", Gap)
            .SetIfNotNull("place-items", ItemPlacement?.ToCss());
    }
}

public static class GridExtensions
{
    public static StyleBuilder Grid(this StyleBuilder builder, 
        string templateColumns = "1fr", 
        string templateRows = "1fr", 
        Spacing? gap = null, 
        Grid.Placement? itemPlacement = null)
    {
        return builder
            .Set("display", "grid")
            .Set("grid-template-columns", templateColumns)
            .Set("grid-template-rows", templateRows)
            .SetIfNotNull("gap", gap)
            .SetIfNotNull("place-items", itemPlacement?.ToCss());
    }
}
