using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Components
{
    public abstract class RTBStyleBase : ComponentBase
    {
        [CascadingParameter] private IStyleAppender? App { get; set; }

        protected StyleBuilder StyleBuilder { get; set; } = StyleBuilder.Start;

        [Parameter] public bool Condition { get; set; } = true;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Add(StyleBuilder.Build());
        }

        private void Add(string decls)
        {
            if (Condition && !string.IsNullOrWhiteSpace(decls))
                App?.Append(decls);
        }
    }
}
