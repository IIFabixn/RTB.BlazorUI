using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

public abstract class RTBStyleBase : ComponentBase, IStyleContributor, IAsyncDisposable
{
    [CascadingParameter(Name = nameof(StyleBuilder))] 
    public required StyleBuilder StyleBuilder { get; set; }

    [Parameter] public bool Condition { get; set; } = true;

    private bool _registered;
    protected override void OnInitialized() => UpdateRegistration();
    protected override void OnParametersSet() => UpdateRegistration();

    protected abstract void BuildStyle(StyleBuilder builder);

    public void Contribute(StyleBuilder builder)
    {
        if (!Condition) return;
        BuildStyle(builder);
    }

    private void UpdateRegistration()
    {
        if (Condition && !_registered) { StyleBuilder.Register(this); _registered = true; }
        else if (!Condition && _registered) { StyleBuilder.Unregister(this); _registered = false; }
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_registered) { StyleBuilder.Unregister(this); _registered = false; }
        return ValueTask.CompletedTask;
    }
}
