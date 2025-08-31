using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

public abstract class RTBStyleBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = nameof(StyleBuilder)), EditorRequired] public required StyleBuilder StyleBuilder { get; set; } = null!;

    [Parameter] public bool Condition { get; set; } = true;

    protected override void OnInitialized()
    {
        StyleBuilder.Register(this);
    }

    public abstract IStyleBuilder BuildStyle(IStyleBuilder builder);

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        StyleBuilder.Unregister(this);
        return ValueTask.CompletedTask;
    }
}
