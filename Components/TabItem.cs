using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Components;

public class TabItem : RTBComponent, IDisposable
{
    [CascadingParameter] public TabGroup? TabGroup { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public RenderFragment? HeadContent { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;


    protected override void OnParametersSet()
    {
        TabGroup?.RegisterTabItem(this);
    }
    public void Dispose()
    {
        TabGroup?.UnregisterTabItem(this);
    }
}
