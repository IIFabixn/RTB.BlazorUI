using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Interfaces;

namespace RTB.Blazor.Components;

/// <summary>
/// A component representing a single tab item within a tab group.
/// </summary>
public class TabItem : RTBComponent, IDisposable
{
    /// <summary>
    /// A unique identifier for the tab item.
    /// </summary>
    public readonly Guid Guid = Guid.NewGuid();

    /// <summary>
    /// The tab group that this tab item belongs to.
    /// </summary>
    [CascadingParameter] public IRegister<TabItem>? TabGroup { get; set; }

    /// <summary>
    /// The title of the tab item.
    /// </summary>
    [Parameter, EditorRequired] public required string Title { get; set; } = string.Empty;

    /// <summary>
    /// The content to be displayed when the tab is active.
    /// </summary>
    [Parameter] public RenderFragment? TabContent { get; set; }

    /// <summary>
    /// The child content of the tab item, typically used for defining the tab's label or header.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// <inheritdoc cref="ComponentBase.OnParametersSet"/>
    /// </summary>
    protected override void OnParametersSet()
    {
        TabGroup?.Register(this);
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    public void Dispose()
    {
        TabGroup?.Unregister(this);
        GC.SuppressFinalize(this);
    }
}
