using System;
using System.Collections.Generic;
using System.Diagnostics;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Components;

public class Paper : RTBComponent
{
    [Inject] protected IStyled Styled { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        base.OnParametersSet();
        ComponentClass = await Styled.CssAsync(RTBStyle.Build());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", ClassBuilder.Create("Paper", ComponentClass, Class).Build());
        builder.AddMultipleAttributes(seq++, CapturedAttributes?.Without("class", "style"));
        builder.AddContent(seq++, ChildContent);
        builder.CloseElement();
    }
}
