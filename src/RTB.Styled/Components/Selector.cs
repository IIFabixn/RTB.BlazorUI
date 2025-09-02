using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Blazor component that scopes child style contributions under a specific CSS selector/query.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This component collects child styles into a private <see cref="StyleBuilder"/> instance and, during
    /// <see cref="BuildStyle(StyleBuilder)"/>, appends them to the parent builder as a selector rule
    /// via <see cref="StyleBuilder.Selector(string, System.Action{StyleBuilder})"/>.
    /// </para>
    /// <para>
    /// The <see cref="Query"/> supports the <c>&amp;</c> placeholder to reference the current scope. When <see cref="Query"/>
    /// is null or whitespace, it implicitly becomes <c>&amp;</c>, meaning "use the current scope as-is".
    /// Examples (assuming current scope ".root"):
    /// </para>
    /// <list type="bullet">
    ///   <item><description><c>Query="&amp;:hover"</c> results in selector ".root:hover".</description></item>
    ///   <item><description><c>Query=".child"</c> results in selector ".root .child".</description></item>
    ///   <item><description><c>Query=""</c> (empty) results in selector ".root".</description></item>
    ///   <item><description><c>Query=".a, .b"</c> emits for both ".root .a" and ".root .b".</description></item>
    /// </list>
    /// <para>
    /// Lifecycle:
    /// </para>
    /// <list type="number">
    ///   <item><description>Child components contribute styles into the cascaded inner <see cref="StyleBuilder"/>.</description></item>
    ///   <item><description>On composition, <see cref="BuildStyle(StyleBuilder)"/> calls <c>_inner.Compose()</c> to gather children.</description></item>
    ///   <item><description>The collected styles are appended to the parent via <c>builder.Selector(Query, sb =&gt; sb.Absorb(_inner))</c>.</description></item>
    ///   <item><description><c>_inner.ClearAll()</c> resets the inner builder to avoid leaking state across compositions.</description></item>
    /// </list>
    /// <example>
    /// <code>
    /// &lt;Styled&gt;
    ///   &lt;Selector Query="&amp;:hover"&gt;
    ///     &lt;Set Prop="color" Value="red" /&gt;
    ///   &lt;/Selector&gt;
    ///   &lt;Selector Query=".title, .subtitle"&gt;
    ///     &lt;Set Prop="font-weight" Value="600" /&gt;
    ///   &lt;/Selector&gt;
    /// &lt;/Styled&gt;
    /// </code>
    /// </example>
    /// <seealso cref="StyleBuilder"/>
    /// <seealso cref="StyleBuilder.Selector(string, System.Action{StyleBuilder})"/>
    /// <seealso cref="RTBStyleBase"/>
    /// <seealso cref="RTB.Blazor.Styled.Core.SelectorRule"/>
    /// </remarks>
    public class Selector : RTBStyleBase
    {
        /// <summary>
        /// The child content that contributes style declarations and fragments to this selector's inner builder.
        /// </summary>
        /// <remarks>
        /// The content receives a cascaded <see cref="StyleBuilder"/> instance (private to this component)
        /// so that any nested style components write into the selector-scoped builder instead of the parent.
        /// </remarks>
        [Parameter, EditorRequired] public required RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The CSS selector/query to scope the child styles under.
        /// </summary>
        /// <remarks>
        /// - Supports the <c>&amp;</c> placeholder for the current scope, primarily when the selector starts with <c>&amp;</c>.
        /// - When null or whitespace, defaults to <c>&amp;</c>, i.e., "use the current scope".
        /// - Supports comma-delimited lists (e.g., <c>.a, .b</c>).
        /// </remarks>
        [Parameter] public string Query { get; set; } = string.Empty;

        // Private style builder that accumulates child styles for this selector instance.
        private readonly StyleBuilder _inner = StyleBuilder.Start;

        /// <summary>
        /// Contributes this component's styles to the provided <paramref name="builder"/>.
        /// </summary>
        /// <remarks>
        /// - Resolves <see cref="Query"/> (defaults to <c>&amp;</c> when empty).
        /// - Composes child contributions into the inner builder.
        /// - Appends them to the parent under the resolved selector via <see cref="StyleBuilder.Selector(string, System.Action{StyleBuilder})"/>.
        /// - Clears the inner builder to prevent state leakage.
        /// </remarks>
        /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
        protected override void BuildStyle(StyleBuilder builder)
        {
            var query = string.IsNullOrWhiteSpace(Query) ? "&" : Query;
            _inner.Compose();
            builder.Selector(query, sb => sb.Absorb(_inner));
            _inner.ClearAll();
        }

        /// <summary>
        /// Renders a fixed <see cref="CascadingValue{TValue}"/> that supplies the inner <see cref="StyleBuilder"/>
        /// to <see cref="ChildContent"/> so nested style components write into this selector's scope.
        /// </summary>
        /// <param name="builder">The render tree builder.</param>
        /// <remarks>
        /// Uses <c>IsFixed=true</c> to avoid re-rendering the cascade reference when not necessary.
        /// </remarks>
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
