using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Extensions;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Wraps child styles into a media query using the provided <see cref="BreakPoint"/>.
    /// Collects child styles via a private <see cref="StyleBuilder"/> and appends them
    /// as a single media block to the parent builder.
    /// </summary>
    public class Media : RTBStyleBase
    {
        [Parameter, EditorRequired] public required RenderFragment ChildContent { get; set; }
        [Parameter, EditorRequired] public required BreakPoint BreakPoint { get; set; }

        private readonly StyleBuilder _inner = StyleBuilder.Start;

        protected override void BuildStyle(StyleBuilder builder)
        {
            _inner.Compose();
            builder.Media(BreakPoint.ToQuery(), b =>  b.Absorb(_inner));
            _inner.ClearAll();
        }

        protected override void BuildRenderTree(RenderTreeBuilder renderBuilder)
        {
            // Provide the private StyleBuilder to descendants
            renderBuilder.OpenComponent<CascadingValue<StyleBuilder>>(0);
            renderBuilder.AddAttribute(1, "Value", _inner);
            renderBuilder.AddAttribute(2, "Name", nameof(StyleBuilder));
            renderBuilder.AddAttribute(3, "IsFixed", true);
            renderBuilder.AddAttribute(4, "ChildContent", ChildContent);
            renderBuilder.CloseComponent();
        }
    }
}
