using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Interfaces;

namespace RTB.Blazor.Components;

public class TabItem : RTBComponent, IDisposable
{
    public readonly Guid Guid = Guid.NewGuid();

    [CascadingParameter] public IRegister<TabItem>? TabGroup { get; set; }
    [Parameter, EditorRequired] public required string Title { get; set; } = string.Empty;
    [Parameter] public RenderFragment? TabContent { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override void OnParametersSet()
    {
        TabGroup?.Register(this);
    }

    public void Dispose()
    {
        TabGroup?.Unregister(this);
        GC.SuppressFinalize(this);
    }
}
