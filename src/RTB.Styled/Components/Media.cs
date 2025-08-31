using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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

        private readonly StyleBuilder _builder = StyleBuilder.Start;

        public override StyleBuilder BuildStyle(StyleBuilder builder)
        {
            if (!Condition) return builder;

            var style = _builder.Build();
            return builder.AppendMedia(BreakPoint.ToQuery(), style);
        }

        protected override void BuildRenderTree(RenderTreeBuilder renderBuilder)
        {
            // Provide the private StyleBuilder to descendants
            renderBuilder.OpenComponent<CascadingValue<StyleBuilder>>(0);
            renderBuilder.AddAttribute(1, "Value", _builder);
            renderBuilder.AddAttribute(2, "Name", nameof(StyleBuilder));
            renderBuilder.AddAttribute(2, "IsFixed", true);
            renderBuilder.AddAttribute(3, "ChildContent", ChildContent);
            renderBuilder.CloseComponent();
        }
    }
}
