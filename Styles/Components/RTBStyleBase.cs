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
        [Inject] private IStyleRegistry Registry { get; set; } = default!;
        [CascadingParameter] public string Classname { get; set; } = string.Empty;
        [Parameter] public bool Condition { get; set; } = true;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrEmpty(Classname) || !Condition) return;
            Registry.InjectInto(BuildStyle().Build(), Classname);
        }

        protected abstract StyleBuilder BuildStyle();
    }
}
