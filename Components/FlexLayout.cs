
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;

namespace RTB.BlazorUI.Components;
public class FlexLayout : RTBComponent
{
    [Parameter] public bool IsHorizontal { get; set; } = false;

    [Parameter] public bool IsVertical { get; set; } = false;
    [Parameter] public bool FullHeight { get; set; } = false;

    [Parameter] public string Element { get; set; } = "div";

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, CapturedAttributes.Where(kvp => kvp.Key != "class"));
        builder.AddAttribute(2, "class", ClassBuilder.Create("flex").AppendIf("h-full", FullHeight).AppendIf("flex-row", IsHorizontal).AppendIf("flex-col", IsVertical).Append(CapturedAttributes.GetValueOrDefault<string>("class")).Build());
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
