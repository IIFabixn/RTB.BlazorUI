using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Extensions;
using RTB.Blazor.Styled.Helper;
using System.Xml.Linq;

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

        private readonly StyleBuilder _inner = StyleBuilder.Start;

        protected override void BuildStyle(StyleBuilder builder)
        {
            var query = string.IsNullOrWhiteSpace(Query) ? "&" : Query;
            _inner.Compose();
            builder.Selector(query, sb => sb.Absorb(_inner));
            _inner.ClearAll();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<StyleBuilder>>(0);
            builder.AddAttribute(1, "Value", _inner);
            builder.AddAttribute(2, "Name", nameof(StyleBuilder));
            builder.AddAttribute(3, "IsFixed", true);
            builder.AddAttribute(4, "ChildContent", ChildContent);
            builder.CloseComponent();
        }
    }
}
