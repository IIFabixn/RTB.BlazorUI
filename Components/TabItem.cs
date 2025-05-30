using System;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Interfaces;

namespace RTB.BlazorUI.Components;

public class TabItem : RTBComponent, IDisposable
{
    public readonly Guid Guid = Guid.NewGuid();

    [Inject] protected IStyled Styled { get; set; } = default!;

    [CascadingParameter] public IRegister<TabItem>? TabGroup { get; set; }
    [Parameter, EditorRequired] public required string Title { get; set; } = string.Empty;
    [Parameter] public RenderFragment? TabContent { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override void OnParametersSet()
    {
        TabGroup?.Register(this);
    }

    protected override async Task OnInitializedAsync()
    {
        ComponentClass = await Styled.CssAsync(RTBStyle.Build());
    }

    public void Dispose()
    {
        TabGroup?.Unregister(this);
        GC.SuppressFinalize(this);
    }
}
