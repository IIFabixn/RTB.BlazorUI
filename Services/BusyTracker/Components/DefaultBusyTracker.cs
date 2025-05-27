using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTB.BlazorUI.Components;
using BlazorStyled;
using RTB.BlazorUI.Helper;

namespace RTB.BlazorUI.Services.BusyTracker.Components
{
    public class DefaultBusyTracker : RTBComponent
    {
        [Inject] protected IStyled Styled { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            ComponentClass = await Styled.CssAsync(StyleBuilder.Create()
                .Append("height", "100%")
                .Append("width", "100%")
                .Append("display", "grid")
                .Append("place-items", "center")
                .Build());
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", ComponentClass);
            builder.AddContent(2, (_builder) => {
                _builder.OpenElement(0, "span");
                _builder.AddAttribute(1, "class", "inline text-red-400 animate-spin");
                _builder.AddContent(2, ".");
                _builder.CloseElement();
            });
            builder.CloseElement();
        }
    }
}
