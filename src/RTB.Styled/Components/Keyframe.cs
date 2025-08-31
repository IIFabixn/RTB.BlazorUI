using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Represents a single frame inside an Animation's @keyframes block.
/// </summary>
public class Keyframe : RTBStyleBase
{
    /// <summary>
    /// "from" | "to" | "0%" | "50%" | "100%" ...
    /// </summary>
    [Parameter, EditorRequired] public required string Offset { get; set; }

    [CascadingParameter(Name = nameof(AnimationName))] public string AnimationName { get; set; } = string.Empty;

    [Parameter, EditorRequired] public required RenderFragment ChildContent { get; set; }

    private readonly StyleBuilder _builder = StyleBuilder.Start;

    public override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        if (!Condition) return builder;

        var style = _builder.Build();
        builder.AppendKeyFrame(AnimationName, Offset.Trim(), style);
        return builder;
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
