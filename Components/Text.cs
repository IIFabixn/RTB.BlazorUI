using System;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;

namespace RTB.BlazorUI.Components;

public class Text : RTBComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public TextStyle Style { get; set; } = new();
    
    [Parameter] public string Element { get; set; } = "h1";
    [Parameter] public string? FontSize { get; set; }
    [Parameter] public string? FontWeight { get; set; }
    [Parameter] public string? LineHeight { get; set; }
    [Parameter] public string? Color { get; set; }
    
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;
        builder.OpenComponent<Styled>(0);
        builder.AddAttribute(seq++, nameof(Styled.Classname), ComponentClass);
        builder.AddAttribute(seq++, nameof(Styled.ClassnameChanged), EventCallback.Factory.Create(this, (string newClassName) => ComponentClass = newClassName));
        builder.AddContent(seq++, StyleBuilder.Create()
            .Append("font-size", FontSize ?? Style.FontSize)
            .Append("font-weight", FontWeight ?? Style.FontWeight)
            .Append("line-height", LineHeight ?? Style.LineHeight)
            .Append("color", Color ?? Style.Color)
            .Build());
        builder.CloseComponent();

        builder.OpenElement(seq++, Element);
        builder.AddAttribute(seq++, "class", ComponentClass);
        builder.AddMultipleAttributes(seq++, CapturedAttributes.Without("class"));
        builder.AddContent(seq++, ChildContent);
        builder.CloseElement();
    }
}

public class TextStyle
{
    public string FontSize { get; set; } = "1rem";
    public string FontWeight { get; set; } = "normal";
    public string LineHeight { get; set; } = "1.5";
    public string Color { get; set; } = "inherit";
}