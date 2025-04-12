
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
        builder.AddAttribute(2, "class", $"flex {(FullHeight ? "h-full" : string.Empty)} {(IsHorizontal ? "flex-row" : string.Empty)} {(IsVertical ? "flex-col" : string.Empty)} {(CapturedAttributes.TryGetValue("class", out var classes) ? classes : string.Empty)}");
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
