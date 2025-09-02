using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// A generic style component that writes an arbitrary CSS declaration to the current style scope.
/// </summary>
/// <remarks>
/// - <see cref="Property"/> is required and should be a valid CSS property name (e.g., "gap", "background-color", "--my-var").
/// - When <see cref="Value"/> is null or whitespace, nothing is emitted.
/// - Whitespace-only or null property names are ignored.
/// - This component contributes to a cascading <see cref="StyleBuilder"/> provided by an ancestor style root.
/// </remarks>
/// <example>
/// Component usage:
/// <code language="razor">
/// @* Emits: gap: 1rem; *@
/// <Other Property="gap" Value="1rem" />
///
/// @* Emits a CSS custom property *@
/// <Other Property="--card-radius" Value="12px" />
/// </code>
///
/// Imperative builder usage (equivalent behavior):
/// <code language="csharp">
/// builder.Other("gap", "1rem");
/// builder.Other("--card-radius", "12px");
/// </code>
/// </example>
public class Other : RTBStyleBase
{
    /// <summary>
    /// The CSS property name to emit (e.g., "gap", "background-color", "--my-var").
    /// </summary>
    /// <remarks>
    /// Must not be null or whitespace. Marked as <see cref="EditorRequiredAttribute"/>.
    /// </remarks>
    [Parameter, EditorRequired] public string Property { get; set; } = string.Empty;

    /// <summary>
    /// The CSS value to assign to <see cref="Property"/> (e.g., "1rem", "#333", "var(--x)").
    /// </summary>
    /// <remarks>
    /// If null or whitespace, no declaration is emitted.
    /// </remarks>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// Contributes a single CSS declaration if both <see cref="Property"/> and <see cref="Value"/> are set.
    /// </summary>
    /// <param name="builder">The style builder receiving the declaration.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Other(Property, Value);
    }
}

/// <summary>
/// Extension helpers for emitting arbitrary CSS declarations using <see cref="StyleBuilder"/>.
/// </summary>
public static class OtherExtensions
{
    /// <summary>
    /// Adds a custom CSS property and value to the style builder.
    /// </summary>
    /// <param name="builder">The target <see cref="StyleBuilder"/>.</param>
    /// <param name="property">The CSS property name (e.g., "gap", "--my-var"). Ignored when null or whitespace.</param>
    /// <param name="value">The CSS value to assign. Ignored when null or whitespace.</param>
    /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
    /// <remarks>
    /// No declaration is added when either <paramref name="property"/> or <paramref name="value"/> is null or whitespace.
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var css = StyleBuilder.Start
    ///     .Other("gap", "1rem")
    ///     .Other("--ring-color", "hsl(210 100% 50%)");
    /// </code>
    /// </example>
    public static StyleBuilder Other(this StyleBuilder builder, string property, string? value)
    {
        return builder.SetIfNotNull(property, value);
    }
}
