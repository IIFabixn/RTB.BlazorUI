using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;
namespace RTB.BlazorUI.Components
{
    /// <summary>
    /// Box is a simple wrapper component that can be used to simply create layouts.
    /// </summary>
    public class Box : RTBComponent
    {
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter] public bool Grid { get; set; } = false;
        [Parameter] public bool Flex { get; set; } = false;
        [Parameter] public bool FullHeight { get; set; } = false;
        [Parameter] public bool FullWidth { get; set; } = false;
        [Parameter] public string Element { get; set; } = "div";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;
            builder.OpenElement(seq++, Element);
            builder.AddAttribute(seq++, "class", ClassBuilder.Create()
                .AppendIf("h-full", FullHeight)
                .AppendIf("w-full", FullWidth)
                .AppendIf("grid", Grid)
                .AppendIf("flex", Flex)
                .Append(CapturedAttributes.GetValueOrDefault<string>("class")) // additional classes
                .Build());
            builder.AddMultipleAttributes(seq++, CapturedAttributes.Without("class"));
            builder.AddContent(seq++, ChildContent);
            builder.CloseElement();
        }
    }
}
