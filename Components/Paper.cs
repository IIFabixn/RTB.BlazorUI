using System;
using System.Collections.Generic;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;

namespace RTB.BlazorUI.Components;

public class Paper : RTBComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public PaperStyle Style { get; set; } = new();
    [Parameter] public string? BackgroundColor { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;
        builder.OpenComponent<Styled>(seq++);
        builder.AddAttribute(seq++, nameof(Styled.Classname), ComponentClass);
        builder.AddAttribute(seq++, nameof(Styled.ClassnameChanged), EventCallback.Factory.Create(this, (string newClassName) => ComponentClass = newClassName));
        builder.AddContent(seq++, StyleBuilder.Create()
            .Append("background-color", BackgroundColor ?? Style.BackgroundColor)
            .Build());
        builder.CloseComponent();

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", ClassBuilder.Create(ComponentClass).Append(CapturedAttributes.GetValueOrDefault<string>("class")).Build());
        builder.AddMultipleAttributes(seq++, CapturedAttributes.Without("class"));
        builder.AddContent(seq++, ChildContent);
        builder.CloseElement();
    }
}

public class PaperStyle
{
    public string BackgroundColor { get; set; } = "white";
}
