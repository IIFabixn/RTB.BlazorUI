using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Extensions;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Represents a single keyframe entry inside an ancestor animation's <c>@keyframes</c> block.
/// </summary>
/// <remarks>
/// - An ancestor component is expected to provide a cascading <see cref="AnimationName"/> (the name of the keyframes block).<br/>
/// - Child content contributes CSS declarations for this specific frame via the cascading <see cref="StyleBuilder"/> context supplied by this component.<br/>
/// - When composed, the collected declarations are emitted as a <see cref="KeyframeFrame"/> at the specified <see cref="Offset"/> inside the named keyframes.
/// </remarks>
/// <example>
/// Example (conceptual):
/// <code language="razor">
/// <!-- Ancestor component establishes AnimationName="pulse" and a root StyleBuilder -->
/// <Keyframe Offset="0%">
///     @* Contribute declarations for the 0% frame (e.g., opacity: 0.2; transform: scale(0.9);) *@
/// </Keyframe>
/// <Keyframe Offset="100%">
///     @* Contribute declarations for the 100% frame (e.g., opacity: 1; transform: scale(1);) *@
/// </Keyframe>
/// </code>
/// Results in CSS similar to:
/// <code>
/// @keyframes pulse{0%{opacity:0.2;transform:scale(0.9);}100%{opacity:1;transform:scale(1);}}
/// </code>
/// </example>
public class Keyframe : RTBStyleBase
{
    /// <summary>
    /// The keyframe offset selector for this frame.
    /// </summary>
    /// <remarks>
    /// Accepts standard keyframe selectors such as:
    /// - "from" (equivalent to "0%")
    /// - "to" (equivalent to "100%")
    /// - Percentage values like "0%", "50%", "100%".
    /// The value is not validated; provide a valid CSS keyframe selector.
    /// </remarks>
    [Parameter, EditorRequired]
    public required string Offset { get; set; }

    /// <summary>
    /// The name of the <c>@keyframes</c> block to which this frame contributes.
    /// </summary>
    /// <remarks>
    /// Provided via cascading parameter by an ancestor animation component. When empty or whitespace,
    /// this component contributes nothing during composition.
    /// </remarks>
    [CascadingParameter(Name = nameof(AnimationName))]
    public string AnimationName { get; set; } = string.Empty;

    /// <summary>
    /// Child content that contributes CSS declarations for this frame.
    /// </summary>
    /// <remarks>
    /// The child content receives a cascading <see cref="StyleBuilder"/> instance that targets this frame only.
    /// Use RTB Styled child components that write to the provided builder (e.g., components that call
    /// <see cref="StyleBuilder.Set(string, string)"/> or add nested rules) to define declarations for the frame.
    /// </remarks>
    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Collects declarations emitted by <see cref="ChildContent"/> for this keyframe.
    /// </summary>
    private readonly StyleBuilder _frameBuilder = StyleBuilder.Start;

    /// <summary>
    /// Contributes a <see cref="KeyframeFrame"/> into the named <c>@keyframes</c> block when any declarations were produced by <see cref="_frameBuilder"/>.
    /// </summary>
    /// <param name="builder">The ambient style builder composing the overall stylesheet.</param>
    /// <remarks>
    /// Workflow:
    /// 1) If <see cref="AnimationName"/> is null/whitespace, no work is performed.
    /// 2) Compose the frame builder to gather declarations from <see cref="ChildContent"/>.
    /// 3) If the frame builder's base declarations are empty, skip emission.
    /// 4) Otherwise, add/modify the keyframes block named <see cref="AnimationName"/> and insert a frame at <see cref="Offset"/>.
    /// 5) Clear the frame builder to avoid leaking declarations into subsequent compositions.
    /// </remarks>
    protected override void BuildStyle(StyleBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(AnimationName)) return;

        _frameBuilder.Compose();
        if (_frameBuilder.Base.IsEmpty) return;

        builder.Keyframes(AnimationName, k =>
        {
            var frame = new KeyframeFrame(Offset);
            frame.Declarations.Join(_frameBuilder.Base);
            k.Add(frame);
        });

        _frameBuilder.ClearAll();
    }

    /// <summary>
    /// Supplies a fixed cascading <see cref="StyleBuilder"/> to <see cref="ChildContent"/> so it can emit declarations for this keyframe.
    /// </summary>
    /// <param name="builder">The Blazor <see cref="RenderTreeBuilder"/>.</param>
    /// <remarks>
    /// The cascading value is marked as fixed to avoid unnecessary re-renders and guarantees that all child contributors
    /// target the same frame-local builder instance.
    /// </remarks>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<StyleBuilder>>(0);
        builder.AddAttribute(1, "Value", _frameBuilder);
        builder.AddAttribute(2, "Name", nameof(StyleBuilder));
        builder.AddAttribute(3, "IsFixed", true);
        builder.AddAttribute(4, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
