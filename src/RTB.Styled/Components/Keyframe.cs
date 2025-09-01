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

    private readonly StyleBuilder _frameBuilder = StyleBuilder.Start;

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
