using System;
using System.Collections.Generic;
using System.Diagnostics;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;

namespace RTB.BlazorUI.Components;

public class Paper : RTBComponent
{
    [Inject] protected IStyled Styled { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public PaperStyle Style { get; set; } = new();
    [Parameter] public string? BackgroundColor { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        base.OnParametersSet();
        ComponentClass = await Styled.CssAsync(StyleBuilder.Create()
        .Append("background-color", BackgroundColor ?? Style.BackgroundColor)
        .Build());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;

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
