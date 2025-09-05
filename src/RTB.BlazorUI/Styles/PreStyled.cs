using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styles;

/// <summary>
/// A minimal <see cref="RTBStyleBase"/> implementation intended as a placeholder
/// for pre-styled or externally composed styles.
/// </summary>
/// <remarks>
/// - This component currently does not contribute any styles; <see cref="BuildStyle(StyleBuilder)"/> is a no-op.<br/>
/// - It can be used as a stable anchor in the render tree where a future style contribution
///   may be injected or where a derived component can override behavior.<br/>
/// - The optional <see cref="Style"/> parameter is reserved for scenarios where an
///   <see cref="IStyle"/> may later be absorbed into the cascading <see cref="StyleBuilder"/>.
///   In this implementation it is not consumed.
/// </remarks>
/// <example>
/// Usage as a placeholder within a style scope:
/// <code>
/// <StyleRoot>
///     <PreStyled />
/// </StyleRoot>
/// </code>
/// </example>
public class PreStyled : RTBStyleBase
{
    /// <summary>
    /// An pre-built style instance that could be forwarded or absorbed into the
    /// current style composition.
    /// </summary>
    [Parameter] public IStyle? Style { get; set; }

    /// <summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// </summary>
    /// <param name="builder">The cascading <see cref="StyleBuilder"/>.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        if (Style is null) return;
        builder.Absorb(Style.ToStyle());
    }
}
