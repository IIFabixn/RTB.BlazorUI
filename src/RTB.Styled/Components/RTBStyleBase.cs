using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Base component for contributing styles to a <see cref="Core.StyleBuilder"/> within a Blazor render tree.
/// </summary>
/// <remarks>
/// <para>
/// RTBStyleBase participates in style composition via a cascading <see cref="StyleBuilder"/> parameter. When
/// <see cref="Condition"/> is true, the instance auto-registers itself with the builder during initialization
/// and parameter updates, and unregisters when the condition becomes false or the component is disposed.
/// </para>
/// <para>
/// Inheritors implement <see cref="BuildStyle(StyleBuilder)"/> to describe CSS declarations, selectors, groups,
/// and keyframes. The contribution is gated by <see cref="Condition"/> and invoked by the builder during composition.
/// </para>
/// </remarks>
public abstract class RTBStyleBase : ComponentBase, IStyleContributor, IAsyncDisposable
{
    /// <summary>
    /// The cascading style builder into which this component contributes styles.
    /// </summary>
    /// <remarks>
    /// This value must be provided by an ancestor component that establishes a style scope (e.g., a style root).
    /// The registration lifecycle is managed automatically based on <see cref="Condition"/>.
    /// </remarks>
    [CascadingParameter(Name = nameof(StyleBuilder))]
    public required StyleBuilder StyleBuilder { get; set; }

    /// <summary>
    /// Controls whether this component is registered with the <see cref="StyleBuilder"/> and contributes styles.
    /// </summary>
    /// <remarks>
    /// - When set to true (default), the instance registers with the builder and participates in composition.<br/>
    /// - When set to false, the instance unregisters and contributes nothing.<br/>
    /// Toggling this parameter at runtime updates the registration accordingly.
    /// </remarks>
    [Parameter] public bool Condition { get; set; } = true;

    // Tracks whether this instance is currently registered with the StyleBuilder.
    private bool _registered;

    /// <summary>
    /// Initializes the component and ensures registration state reflects the current <see cref="Condition"/>.
    /// </summary>
    protected override void OnInitialized() => UpdateRegistration();

    /// <summary>
    /// Ensures registration state reflects the current <see cref="Condition"/> whenever parameters change.
    /// </summary>
    protected override void OnParametersSet() => UpdateRegistration();

    /// <summary>
    /// Implementors define style declarations and nested rules here.
    /// </summary>
    /// <param name="builder">The builder to receive declarations, selectors, groups, and keyframes.</param>
    /// <remarks>
    /// This method is called by the infrastructure when <see cref="IStyleContributor.Contribute(StyleBuilder)"/> is invoked
    /// by the owning <see cref="StyleBuilder"/> during composition, provided <see cref="Condition"/> is true.
    /// Typical usage:
    /// <list type="bullet">
    ///   <item><description>Use <c>builder.Set</c> to add base declarations.</description></item>
    ///   <item><description>Use <c>builder.Selector</c> to add nested selector rules.</description></item>
    ///   <item><description>Use <c>builder.Media</c>, <c>Supports</c>, or <c>Container</c> for group rules.</description></item>
    ///   <item><description>Use <c>builder.Keyframes</c> to define animations.</description></item>
    /// </list>
    /// Avoid long-running or stateful operations; this method may be invoked multiple times during recomposition.
    /// </remarks>
    protected abstract void BuildStyle(StyleBuilder builder);

    /// <summary>
    /// Contributes style fragments to the provided <paramref name="builder"/> when <see cref="Condition"/> is true.
    /// </summary>
    /// <param name="builder">The style builder currently composing styles.</param>
    void IStyleContributor.Contribute(StyleBuilder builder)
    {
        if (!Condition) return;
        BuildStyle(builder);
    }

    /// <summary>
    /// Registers or unregisters this contributor with the <see cref="StyleBuilder"/> based on <see cref="Condition"/>.
    /// </summary>
    private void UpdateRegistration()
    {
        if (Condition && !_registered) { StyleBuilder.Register(this); _registered = true; }
        else if (!Condition && _registered) { StyleBuilder.Unregister(this); _registered = false; }
    }

    /// <summary>
    /// Unregisters this contributor from the <see cref="StyleBuilder"/> and suppresses finalization.
    /// </summary>
    /// <remarks>
    /// Safe to call multiple times. After disposal, no further contributions will occur.
    /// </remarks>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_registered) { StyleBuilder.Unregister(this); _registered = false; }
        return ValueTask.CompletedTask;
    }
}
