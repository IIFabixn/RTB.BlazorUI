using System;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Theme.Styles;

namespace RTB.BlazorUI.Components;

public class Text : RTBComponent
{
    [Inject] protected IStyled Styled { get; set; } = default!;

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    [Parameter] public TextStyle? TextStyle { get; set; }
    
    [Parameter] public string Element { get; set; } = "h1";
    [Parameter] public string? TextAlign { get; set; }
    [Parameter] public string? FontSize { get; set; }
    [Parameter] public string? FontWeight { get; set; }
    [Parameter] public string? LineHeight { get; set; }
    [Parameter] public string? TextDecoration { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        ComponentClass = await Styled.CssAsync(RTBStyle
            .AppendIfNotEmpty("text-align", TextAlign)
            .AppendIfNotEmpty("font-size", FontSize ?? TextStyle?.FontSize)
            .AppendIfNotEmpty("font-weight", FontWeight ?? TextStyle?.FontWeight)
            .AppendIfNotEmpty("line-height", LineHeight ?? TextStyle?.LineHeight)
            .AppendIfNotEmpty("text-decoration", TextDecoration ?? TextStyle?.TextDecoration)
            .AppendIfNotEmpty("color", Color?.Hex ?? TextStyle?.Color)
            .Build()
        );
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {        
        var seq = 0;

        builder.OpenElement(seq++, Element);
        builder.AddAttribute(seq++, "class", ClassBuilder.Create("Text", ComponentClass).Build());
        builder.AddMultipleAttributes(seq++, CapturedAttributes?.Without("class"));
        builder.AddContent(seq++, ChildContent);
        builder.CloseElement();
    }
}
