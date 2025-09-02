using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Represents a mutable set of CSS declarations (property/value pairs) destined for the writer's current selector.
    /// </summary>
    /// <remarks>
    /// - Properties and values are trimmed on insert; null or whitespace-only inputs are ignored.<br/>
    /// - Adding the same property more than once overwrites the previous value (last-write-wins).<br/>
    /// - <see cref="Join(IEnumerable{KeyValuePair{string, string}})"/> merges another sequence using the same add rules.<br/>
    /// - <see cref="Emit(ScopedWriter)"/> writes a rule block for <see cref="ScopedWriter.CurrentSelector"/> and does nothing when empty.<br/>
    /// - Emission order follows the dictionary's enumeration order. While .NET currently preserves insertion order for <see cref="Dictionary{TKey, TValue}"/>, callers should not rely on a specific order.<br/>
    /// - No validation or escaping of CSS is performed; callers must provide valid CSS identifiers and values.<br/>
    /// - This type is not thread-safe.
    /// </remarks>
    public sealed class DeclarationSet : IStyleFragment, IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _decls = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets a value indicating whether the set contains no declarations.
        /// </summary>
        public bool IsEmpty => _decls.Count == 0;

        /// <summary>
        /// Adds or replaces a CSS declaration.
        /// </summary>
        /// <param name="prop">The CSS property name. Null or whitespace is ignored.</param>
        /// <param name="value">The CSS value. Null or whitespace is ignored.</param>
        /// <remarks>
        /// Both <paramref name="prop"/> and <paramref name="value"/> are trimmed. If either is null/whitespace, the call is ignored.
        /// When the same property is added multiple times, the last value wins.
        /// </remarks>
        public void Add(string prop, string value)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value)) return;
            _decls[prop.Trim()] = value.Trim(); // last-write-wins
        }

        /// <summary>
        /// Merges a sequence of declarations into this set using last-write-wins semantics.
        /// </summary>
        /// <param name="source">The source sequence of property/value pairs. If null, the call is a no-op.</param>
        /// <remarks>
        /// Each pair is processed via <see cref="Add(string, string)"/>; null or whitespace keys/values are ignored.
        /// </remarks>
        public void Join(IEnumerable<KeyValuePair<string, string>> source)
        {
            if (source == null) return;
            foreach (var (k, v) in source) Add(k, v);
        }

        /// <summary>
        /// Emits a CSS rule block for the current selector into the provided writer.
        /// </summary>
        /// <param name="w">The writer that receives the CSS output. Must not be null.</param>
        /// <remarks>
        /// - No output is written if the set is empty.<br/>
        /// - Uses <see cref="ScopedWriter.CurrentSelector"/> for the selector and writes all current declarations as-is.<br/>
        /// - This method does not clear the set after emission; callers manage lifecycle as needed.
        /// </remarks>
        public void Emit(ScopedWriter w)
        {
            if (IsEmpty) return;
            w.WriteRuleBlock(w.CurrentSelector, _decls);
        }

        /// <summary>
        /// Removes all declarations from the set.
        /// </summary>
        public void Clear() => _decls.Clear();

        /// <summary>
        /// Returns an enumerator that iterates through the declarations in this set.
        /// </summary>
        /// <returns>An enumerator over property/value pairs.</returns>
        /// <remarks>
        /// The enumeration order matches the underlying dictionary's enumeration order.
        /// </remarks>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _decls.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
