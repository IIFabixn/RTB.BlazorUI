using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

public abstract class RTBStyleBase : ComponentBase
{
    [CascadingParameter, EditorRequired] public required StyleBuilder StyleBuilder { get; set; }

    [Parameter] public bool Condition { get; set; } = true;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Condition)
            BuildStyle(StyleBuilder);
    }

    protected abstract StyleBuilder BuildStyle(StyleBuilder builder);
}
