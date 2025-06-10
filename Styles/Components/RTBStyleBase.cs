using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Components
{
    public abstract class RTBStyleBase : ComponentBase
    {
        [CascadingParameter] private StyleBuilder StyleBuilder { get; set; } = default!;
        
        [Parameter] public bool Condition { get; set; } = true;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!Condition) return;
            BuildStyle(StyleBuilder);
        }

        protected abstract StyleBuilder BuildStyle(StyleBuilder builder);
    }
}
