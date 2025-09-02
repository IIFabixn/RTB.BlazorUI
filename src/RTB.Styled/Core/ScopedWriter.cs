using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Provides a minimal, allocation-friendly writer for emitting scoped CSS strings.
    /// Maintains a selector stack to support nested emission contexts without copying strings.
    /// </summary>
    /// <remarks>
    /// - This type does not validate or escape CSS. Callers must provide valid CSS selectors and declarations.
    /// - The root selector is pushed at construction and remains at the base of the stack until disposal/end of usage.
    /// - Designed for high-throughput string building scenarios; all writes append directly to the provided <see cref="StringBuilder"/>.
    /// </remarks>
    public sealed class ScopedWriter
    {
        private readonly StringBuilder _sb;
        private readonly Stack<string> _selectorStack = new();

        /// <summary>
        /// Gets the selector at the top of the selector stack.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if accessed after the internal stack has been fully unwound (should not occur in normal usage).
        /// </exception>
        public string CurrentSelector => _selectorStack.Peek();

        /// <summary>
        /// Initializes a new instance of <see cref="ScopedWriter"/> using the provided <see cref="StringBuilder"/> and root selector.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to which CSS will be appended. Must not be null.</param>
        /// <param name="rootSelector">The root CSS selector to seed the selector stack. Must not be null.</param>
        /// <remarks>
        /// The constructor does not write anything immediately; it only establishes the initial scope.
        /// </remarks>
        public ScopedWriter(StringBuilder sb, string rootSelector)
        {
            _sb = sb;
            _selectorStack.Push(rootSelector);
        }

        /// <summary>
        /// Temporarily pushes a selector onto the stack, executes the provided <paramref name="emit"/> action, then restores the previous selector.
        /// </summary>
        /// <param name="selector">The selector to push for the duration of <paramref name="emit"/>.</param>
        /// <param name="emit">An action that writes within the new selector scope.</param>
        /// <remarks>
        /// Uses a try/finally to guarantee the selector is popped even if <paramref name="emit"/> throws.
        /// </remarks>
        public void WithSelector(string selector, Action emit)
        {
            _selectorStack.Push(selector);
            try { emit(); } finally { _selectorStack.Pop(); }
        }

        /// <summary>
        /// Appends the specified string to the underlying <see cref="StringBuilder"/> without modification.
        /// </summary>
        /// <param name="s">The string to append.</param>
        /// <remarks>
        /// No validation or escaping is performed.
        /// </remarks>
        public void Write(string s) => _sb.Append(s);

        /// <summary>
        /// Writes a CSS rule block of the form: <c>{selector}{{k:v;...}}</c>.
        /// </summary>
        /// <param name="selector">The CSS selector for the rule block.</param>
        /// <param name="decls">A read-only dictionary of CSS declarations (property/value pairs).</param>
        /// <remarks>
        /// - Keys are written as-is as property names; values are written as-is as property values.
        /// - No whitespace or formatting is added beyond curly braces and semicolons to minimize allocations.
        /// </remarks>
        public void WriteRuleBlock(string selector, IReadOnlyDictionary<string, string> decls)
        {
            _sb.Append(selector).Append('{');
            WriteDeclarations(decls);
            _sb.Append('}');
        }

        /// <summary>
        /// Writes a sequence of CSS declarations in the form <c>prop:value;</c> for each pair.
        /// </summary>
        /// <param name="decls">A read-only dictionary of CSS declarations (property/value pairs).</param>
        /// <remarks>
        /// - Declarations are emitted in the dictionary's enumeration order.
        /// - No additional whitespace or escaping is applied.
        /// </remarks>
        public void WriteDeclarations(IReadOnlyDictionary<string, string> decls)
        {
            foreach (var kv in decls)
            {
                _sb.Append(kv.Key).Append(':').Append(kv.Value).Append(';');
            }
        }
    }
}
