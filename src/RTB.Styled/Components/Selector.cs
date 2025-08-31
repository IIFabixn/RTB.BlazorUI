using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Wraps child styles under a specific CSS selector/query.
    /// Collects child styles via a private <see cref="StyleBuilder"/> and appends them
    /// to the parent builder as "Query { ... }".
    /// </summary>
    public class Selector : RTBStyleBase
    {
        [Parameter, EditorRequired] public required RenderFragment ChildContent { get; set; }
        [Parameter] public string Query { get; set; } = string.Empty;

        private readonly StyleBuilder _builder = StyleBuilder.Start;

        public override IStyleBuilder BuildStyle(IStyleBuilder builder)
        {
            if (!Condition) return builder;

            var style = _builder.Build();
            if (string.IsNullOrEmpty(style)) return builder;
            return builder.AppendSelector(Query, style);
        }

        protected override void BuildRenderTree(RenderTreeBuilder renderBuilder)
        {
            // Provide the private StyleBuilder to descendants
            renderBuilder.OpenComponent<CascadingValue<StyleBuilder>>(0);
            renderBuilder.AddAttribute(1, "Value", _builder);
            renderBuilder.AddAttribute(2, "Name", nameof(StyleBuilder));
            renderBuilder.AddAttribute(3, "IsFixed", true);
            renderBuilder.AddAttribute(4, "ChildContent", ChildContent);
            renderBuilder.CloseComponent();
        }
    }
}
