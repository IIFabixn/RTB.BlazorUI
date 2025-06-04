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
        [CascadingParameter] protected StyleBuilder StyleBuilder { get; set; } = default!;

        [Parameter] public Func<bool>? Condition { get; set; }
    }
}
