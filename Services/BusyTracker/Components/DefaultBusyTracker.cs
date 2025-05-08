using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.BusyTracker.Components
{
    public class DefaultBusyTracker : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", " h-full w-full grid place-items-center");
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
