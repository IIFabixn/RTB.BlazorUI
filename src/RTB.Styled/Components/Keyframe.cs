using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Extensions;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Represents a single frame inside an Animation's @keyframes block.
/// </summary>
public class Keyframe : RTBStyleBase
{
    /// <summary>
    /// "from" | "to" | "0%" | "50%" | "100%" ...
    /// </summary>
    [Parameter, EditorRequired]
    public required string Offset { get; set; }

    [CascadingParameter(Name = nameof(AnimationName))] 
    public string AnimationName { get; set; } = string.Empty;

    [Parameter, EditorRequired] 
    public required RenderFragment ChildContent { get; set; }

    private readonly StyleBuilder _builder = StyleBuilder.Start;

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition || string.IsNullOrWhiteSpace(AnimationName)) return builder;
        var parent = builder.AsConcrete();
        var snap = (IStyleSnapshot)_builder;

        // Let Keyframe's own children write into its private builder
        foreach (var child in snap.Children)
            if (child.Condition) child.Contribute(_builder);

        // Re-read updated props and emit a frame on the parent
        snap = (IStyleSnapshot)_builder;
        var decls = snap.Props.Select(p => (p.Key, p.Value)).ToArray();
        if (decls.Length > 0)
            parent.AppendKeyFrame(AnimationName, Offset, decls);

        _builder.Clear();
        return parent;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<StyleBuilder>>(0);
        builder.AddAttribute(1, "Value", _builder);
        builder.AddAttribute(2, "Name", nameof(StyleBuilder));
        builder.AddAttribute(3, "IsFixed", true);
        builder.AddAttribute(4, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
