using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Represents a CSS grouping at-rule (e.g., <c>@media</c>, <c>@supports</c>, <c>@container</c>, <c>@layer</c>)
    /// that wraps child <see cref="IStyleFragment"/> instances inside a single block.
    /// </summary>
    /// <remarks>
    /// - This fragment does not change the current selector scope of the <see cref="ScopedWriter"/>; it only surrounds
    ///   the emitted children with <c>{ ... }</c> after writing the at-rule header.
    /// - If <see cref="Children"/> is empty, <see cref="Emit(ScopedWriter)"/> writes nothing.
    /// - No validation or escaping is performed for <see cref="Kind"/> or <see cref="Prelude"/>; callers must supply valid CSS.
    /// - Whitespace is intentionally minimal to reduce allocations; a single space is inserted between <see cref="Kind"/> and
    ///   <see cref="Prelude"/> only when <see cref="Prelude"/> is non-empty/non-whitespace.
    /// - Output shape (when there are children): <c>{Kind}[ {Prelude}]{{...children...}}</c>
    /// </remarks>
    public sealed class GroupRule : IStyleFragment
    {
        /// <summary>
        /// Gets the at-rule keyword including the leading <c>@</c> (e.g., <c>"@media"</c>, <c>"@supports"</c>, <c>"@container"</c>, <c>"@layer"</c>).
        /// </summary>
        /// <remarks>
        /// This value is not validated; any string will be written as-is.
        /// </remarks>
        public string Kind { get; }      // "@media", "@supports", "@container", "@layer"

        /// <summary>
        /// Gets the at-rule prelude (the text following the at-keyword), such as
        /// <c>screen and (min-width: 992px)</c> for a media query.
        /// </summary>
        /// <remarks>
        /// Never null; if a null value is provided to the constructor it is normalized to <see cref="string.Empty"/>.
        /// When empty or whitespace, no leading space is written between <see cref="Kind"/> and the opening brace.
        /// </remarks>
        public string Prelude { get; }   // e.g., "screen and (min-width: 992px)"

        /// <summary>
        /// Gets the child fragments to be emitted inside the group rule block, in order.
        /// </summary>
        /// <remarks>
        /// Never null. If the collection is empty, nothing is emitted.
        /// </remarks>
        public List<IStyleFragment> Children { get; } = new();

        /// <summary>
        /// Initializes a new <see cref="GroupRule"/> with the specified at-rule <paramref name="kind"/> and <paramref name="prelude"/>.
        /// </summary>
        /// <param name="kind">The at-rule keyword including the leading <c>@</c> (e.g., <c>"@media"</c>).</param>
        /// <param name="prelude">The text following the at-keyword. May be null; null is treated as <see cref="string.Empty"/>.</param>
        public GroupRule(string kind, string prelude)
        {
            Kind = kind; Prelude = prelude ?? string.Empty;
        }

        /// <summary>
        /// Emits the group rule and its children to the provided <see cref="ScopedWriter"/>.
        /// </summary>
        /// <param name="w">The writer that accumulates CSS and maintains selector scope. Must be non-null.</param>
        /// <remarks>
        /// - If there are no children, the method returns without writing anything.
        /// - Writes the at-rule header (<see cref="Kind"/> and optional <see cref="Prelude"/>) followed by a brace-enclosed block.
        /// - Each child is emitted in sequence within the same selector scope as the caller.
        /// - No validation or escaping is performed.
        /// </remarks>
        public void Emit(ScopedWriter w)
        {
            if (Children.Count == 0) return;
            w.Write($"{Kind}{(string.IsNullOrWhiteSpace(Prelude) ? "" : " " + Prelude)}{{");
            foreach (var c in Children) c.Emit(w); // same CurrentSelector
            w.Write("}");
        }
    }
}
