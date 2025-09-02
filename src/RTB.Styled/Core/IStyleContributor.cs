using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Defines a participant that can contribute CSS to a <see cref="StyleBuilder"/> during composition.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// - Register an implementation with <c>StyleBuilder.Register(IStyleContributor)</c>.
    /// - When <c>StyleBuilder.Compose()</c> is invoked, the builder calls <see cref="Contribute(StyleBuilder)"/>
    ///   on each registered contributor to aggregate base declarations, selectors, groups, and keyframes.
    /// Guidance:
    /// - Implementations should be deterministic and idempotent (safe to call multiple times).
    /// - Treat the builder as the sole output channel; avoid external side effects.
    /// - Do not retain references to the provided <see cref="StyleBuilder"/> beyond the call.
    /// </remarks>
    public interface IStyleContributor
    {
        /// <summary>
        /// Contributes style declarations and fragments to the provided <see cref="StyleBuilder"/>.
        /// </summary>
        /// <param name="builder">
        /// The style builder to mutate by adding base declarations, selector rules, group rules, and/or keyframes.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Implementations may throw if <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        void Contribute(StyleBuilder builder);
    }
}
