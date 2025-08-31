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

        private readonly StyleBuilder _builder = StyleBuilder.Start;

        public override IStyleBuilder BuildStyle(IStyleBuilder builder)
        {
            if (!Condition || string.IsNullOrWhiteSpace(Query)) return builder;
            var parent = builder.AsConcrete();
            var snap = (IStyleSnapshot)_builder;

            // Let selector's children write into its private builder
            foreach (var child in snap.Children)
                if (child.Condition) child.Contribute(_builder);

            // Re-read updated props and attach under Query
            snap = (IStyleSnapshot)_builder;
            var decls = snap.Props.Select(p => (p.Key, p.Value)).ToArray();
            if (decls.Length > 0)
                parent.AppendSelector(Query.Trim(), decls);

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
}
