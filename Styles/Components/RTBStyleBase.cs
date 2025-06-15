using Microsoft.AspNetCore.Components;
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
        [CascadingParameter] private StyleBuilder StyleBuilder { get; set; } = StyleBuilder.Start;

        [Parameter] public bool Condition { get; set; } = true;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (StyleBuilder is null)
                throw new InvalidOperationException(
                    $"{GetType().Name} requires a cascading StyleBuilder. " +
                    "Wrap your component tree in <StyleProvider>.");
            if (Condition)
                BuildStyle(StyleBuilder);
        }

        protected abstract StyleBuilder BuildStyle(StyleBuilder builder);
    }
}
