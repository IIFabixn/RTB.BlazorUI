using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Core;
using System;

namespace RTB.Blazor.Styles;

/// <summary>
/// Defines a typed style that can be converted into a <see cref="StyleBuilder"/> for CSS emission.
/// </summary>
/// <remarks>
/// Implementations typically translate component or application state into CSS declarations,
/// selectors, groups (e.g., <c>@media</c>), and keyframes using <see cref="StyleBuilder"/>.
/// Consumers call <see cref="ToStyle"/> to obtain a builder, which can then be composed and emitted
/// as scoped CSS where appropriate.
/// </remarks>
/// <example>
/// Example:
/// <code>
/// public sealed class ButtonStyle : IStyle
/// {
///     public bool Primary { get; init; }
///
///     public StyleBuilder ToStyle()
///     {
///         var sb = StyleBuilder.Start
///             .Set("padding", "0.5rem 1rem")
///             .Set("border", "none")
///             .Set("border-radius", "0.375rem")
///             .SetIf("background", "dodgerblue", Primary)
///             .SetIf("color", "white", Primary);
///
///         // Nested selector relative to the component's scope
///         sb.Selector("&amp;:hover", b => b.Set("filter", "brightness(0.95)"));
///         return sb;
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="StyleBuilder"/>
public interface IStyle
{
    /// <summary>
    /// Creates a <see cref="StyleBuilder"/> representing this style.
    /// </summary>
    /// <returns>
    /// A configured <see cref="StyleBuilder"/> instance to be composed and emitted as CSS.
    /// </returns>
    /// <remarks>
    /// Prefer returning a newly created builder per call to keep results deterministic and side-effect free.
    /// </remarks>
    StyleBuilder ToStyle();
}
