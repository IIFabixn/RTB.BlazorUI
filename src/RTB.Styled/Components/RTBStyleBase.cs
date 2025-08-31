using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

public abstract class RTBStyleBase : ComponentBase, IAsyncDisposable, IStyleContributor
{
    [CascadingParameter(Name = nameof(StyleBuilder))] 
    public required StyleBuilder StyleBuilder { get; set; }

    [Parameter] public bool Condition { get; set; } = true;

    protected override void OnInitialized()
    {
        StyleBuilder.Register(this);
    }

    public void Contribute(StyleBuilder builder) => BuildStyle(builder);

    public abstract IStyleBuilder BuildStyle(IStyleBuilder builder);

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        StyleBuilder.Unregister(this);
        return ValueTask.CompletedTask;
    }
}
