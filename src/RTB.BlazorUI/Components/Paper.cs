using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Services.Theme;

namespace RTB.BlazorUI.Components;

public class Paper : RTBComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", CombineClass("rtb-paper", Class));
        builder.AddContent(seq++, ChildContent);
        builder.CloseElement();
    }
}
