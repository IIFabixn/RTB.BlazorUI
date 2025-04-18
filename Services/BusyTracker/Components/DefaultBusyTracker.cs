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
            builder.AddAttribute(1, "class", "animate-spin inline text-red-400");
            builder.AddContent(2, "Loading...");
            builder.CloseElement();
        }
    }
}
