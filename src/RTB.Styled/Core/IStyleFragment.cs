using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Defines a minimal contract for a style fragment capable of emitting CSS into a <see cref="ScopedWriter"/>.
    /// </summary>
    /// <remarks>
    /// - Fragments should write directly to the provided writer to minimize allocations.
    /// - Use the writer's current selector scope; push nested scopes via <see cref="ScopedWriter.WithSelector(string, Action)"/> when needed.
    /// - Do not capture or store the writer beyond the call to <see cref="Emit(ScopedWriter)"/>.
    /// - No validation or escaping is performed; callers should provide valid selectors and declarations.
    /// - Thread-safety is not required; a single writer should not be used concurrently from multiple threads.
    /// </remarks>
    public interface IStyleFragment
    {
        /// <summary>
        /// Emits CSS for this fragment into the provided <see cref="ScopedWriter"/>.
        /// </summary>
        /// <param name="w">The writer that accumulates CSS and manages selector scope.</param>
        /// <remarks>
        /// Implementations should:
        /// - Write directly via <see cref="ScopedWriter.Write(string)"/>, <see cref="ScopedWriter.WriteRuleBlock(string, System.Collections.Generic.IReadOnlyDictionary{string, string})"/>,
        ///   and <see cref="ScopedWriter.WriteDeclarations(System.Collections.Generic.IReadOnlyDictionary{string, string})"/>.
        /// - Restore any selector scope they alter, preferably by using <see cref="ScopedWriter.WithSelector(string, Action)"/>.
        /// - Treat <paramref name="w"/> as non-null; passing null is invalid and may result in an <see cref="ArgumentNullException"/>.
        /// </remarks>
        void Emit(ScopedWriter w);
    }
}
