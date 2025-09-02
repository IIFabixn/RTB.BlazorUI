using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Represents a CSS-like selector rule that emits its own declarations and any child fragments
    /// relative to the current selector scope provided by a <see cref="ScopedWriter"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       Supports comma-delimited selector lists (e.g., ".a, .b"). Each part is processed independently.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Supports '&amp;' as a placeholder for the current scope, but only when the selector text
    ///       starts with '&amp;' (e.g., "&amp;:hover", "&amp; &gt; .child"). In that case, all '&amp;' are
    ///       replaced with the current scope.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       When the selector part is null, empty, or whitespace, the current scope is used as-is.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       A rule block is written only when <see cref="Declarations"/> is not empty; however,
    ///       <see cref="Children"/> are still emitted within the resolved scope regardless of whether
    ///       declarations exist.
    ///     </description>
    ///   </item>
    /// </list>
    /// <example>
    /// Given a current scope ".parent":
    /// - Selector ".child" resolves to ".parent .child"
    /// - Selector "&amp;:hover" resolves to ".parent:hover"
    /// - Selector "&amp; &gt; .child" resolves to ".parent &gt; .child"
    /// - Selector "" (empty) resolves to ".parent"
    /// - Selector ".a, .b" resolves and emits for ".parent .a" and ".parent .b" independently
    /// </example>
    /// </remarks>
    public sealed class SelectorRule : IStyleFragment
    {
        /// <summary>
        /// Gets the raw selector text for this rule.
        /// May contain a comma-delimited list and/or '&amp;' placeholders.
        /// </summary>
        /// <value>
        /// The unprocessed selector text. When null is provided to the constructor,
        /// this is set to <see cref="string.Empty"/>.
        /// </value>
        public string Selector { get; }

        /// <summary>
        /// The declarations to emit for the resolved selector(s).
        /// </summary>
        /// <remarks>
        /// If <see cref="DeclarationSet.IsEmpty"/> is true, no rule block is written.
        /// Child fragments are still emitted within the resolved scope.
        /// </remarks>
        public DeclarationSet Declarations { get; init; } = new();

        /// <summary>
        /// Nested style fragments that will be emitted within the resolved selector scope.
        /// </summary>
        /// <remarks>
        /// Emitted via <see cref="ScopedWriter.WithSelector(string, Action)"/> for each resolved selector part.
        /// This occurs even when <see cref="Declarations"/> is empty.
        /// </remarks>
        public List<IStyleFragment> Children { get; init; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorRule"/> class.
        /// </summary>
        /// <param name="selector">
        /// The selector to resolve against the current scope; when null, an empty string is used.
        /// </param>
        public SelectorRule(string selector) => Selector = selector ?? string.Empty;

        /// <summary>
        /// Emits CSS for this rule:
        /// - Splits the selector into comma-delimited parts.
        /// - Resolves each part against the current scope.
        /// - Writes a rule block when declarations exist.
        /// - Recursively emits children within the resolved scope.
        /// </summary>
        /// <param name="w">The scoped writer that manages the selector stack and writes CSS.</param>
        public void Emit(ScopedWriter w)
        {
            foreach (var part in SplitSelectors(Selector))
            {
                var resolved = ResolveSelector(part, w.CurrentSelector);
                if (!Declarations.IsEmpty)
                    w.WriteRuleBlock(resolved, Declarations.ToDictionary(kv => kv.Key, kv => kv.Value));

                if (Children.Count > 0)
                    w.WithSelector(resolved, () => { foreach (var c in Children) c.Emit(w); });
            }
        }

        /// <summary>
        /// Splits a comma-delimited selector list, trimming whitespace and skipping empty entries.
        /// </summary>
        /// <param name="s">The selector string which may contain multiple parts.</param>
        /// <returns>An enumerable of individual selector parts.</returns>
        /// <remarks>
        /// Note: This routine does not handle escaping of commas inside attribute selectors or similar;
        /// it assumes plain CSS-like comma separation.
        /// </remarks>
        private static IEnumerable<string> SplitSelectors(string s) =>
            (s ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        /// <summary>
        /// Resolves a selector relative to the current scope.
        /// </summary>
        /// <param name="sel">The selector to resolve. May start with '&amp;' to reference the current scope.</param>
        /// <param name="current">The current selector from the writer's scope.</param>
        /// <returns>
        /// The resolved selector:
        /// - If <paramref name="sel"/> starts with '&amp;', all '&amp;' are replaced by <paramref name="current"/>.
        /// - If <paramref name="sel"/> is null or whitespace, returns <paramref name="current"/>.
        /// - Otherwise, returns "<paramref name="current"/> <paramref name="sel"/>".
        /// </returns>
        /// <example>
        /// ResolveSelector("&amp;:focus", ".root") -> ".root:focus"
        /// ResolveSelector("", ".root") -> ".root"
        /// ResolveSelector(".child", ".root") -> ".root .child"
        /// </example>
        private static string ResolveSelector(string sel, string current) =>
            sel.StartsWith('&')
                ? sel.Replace("&", current)
                : string.IsNullOrWhiteSpace(sel) ? current : $"{current} {sel}";
    }
}
