using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Style;
using RTB.BlazorUI.Styles;

namespace RTB.BlazorUI.Components;

public class Text : RTBComponent
{
    [Inject] protected IStyleRegistry StyleRegistry { get; set; } = default!;

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    [Parameter] public TextStyle? TextStyle { get; set; }
    
    [Parameter] public string Element { get; set; } = "h1";
    [Parameter] public string? TextAlign { get; set; }
    [Parameter] public string? FontSize { get; set; }
    [Parameter] public string? FontWeight { get; set; }
    [Parameter] public string? LineHeight { get; set; }
    [Parameter] public string? TextDecoration { get; set; }
    [Parameter] public string? Color { get; set; }

    private string? ComponentClass { get; set; }
    protected override void OnParametersSet()
    {
        ComponentClass = StyleRegistry.GetOrAdd(StyleBuilder.Start
            .AppendStyle(TextStyle)
            .Append("text-align", TextAlign)
            .Append("font-size", FontSize)
            .Append("font-weight", FontWeight)
            .Append("line-height", LineHeight)
            .Append("text-decoration", TextDecoration)
            .Append("color", Color)
            .Build());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;

        builder.OpenElement(seq++, Element);
        builder.AddAttribute(seq++, "class", ClassBuilder.Create("Text", ComponentClass, Class).Build());
        builder.AddMultipleAttributes(seq++, CapturedAttributes);
        builder.AddContent(seq++, ChildContent);
        builder.CloseElement();
    }
}
