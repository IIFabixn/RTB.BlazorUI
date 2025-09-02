using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes a CSS background-color declaration to the current style scope.
/// </summary>
/// <remarks>
/// - This component participates in style composition via the cascading <see cref="StyleBuilder"/> from <see cref="RTBStyleBase"/>.<br/>
/// - When <see cref="RTBStyleBase.Condition"/> is true and <see cref="Color"/> is non-null, a "background-color" declaration is emitted.<br/>
/// - When <see cref="Color"/> is <c>null</c>, no declaration is produced for background-color.
/// </remarks>
/// <example>
/// As a Blazor style contributor:
/// <code>
/// &lt;Background Color="@RTBColor.FromCss("royalblue")" /&gt;
/// </code>
/// In a style build routine:
/// <code>
/// builder.Background(RTBColor.FromCss("#09f"));
/// </code>
/// </example>
public class Background : RTBStyleBase
{
    /// <summary>
    /// The background color to apply. When <c>null</c>, no "background-color" declaration is generated.
    /// </summary>
    [Parameter] public RTBColor? Color { get; set; }

    // TODO: Consider adding background-image, gradients, and shorthand support (background).

    /// <summary>
    /// Contributes the "background-color" declaration using the configured <see cref="Color"/>.
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// </summary>
    /// <param name="builder">The target <see cref="StyleBuilder"/> to receive the declaration.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Background(Color);
    }
}

/// <summary>
/// Extension helpers for adding background-related CSS to a <see cref="StyleBuilder"/>.
/// </summary>
public static class BackgroundExtensions
{
    /// <summary>
    /// Adds a "background-color" declaration to the builder if <paramref name="color"/> is non-null.
    /// </summary>
    /// <param name="builder">The style builder to mutate.</param>
    /// <param name="color">The color to set. When <c>null</c>, nothing is added.</param>
    /// <returns>The same <see cref="StyleBuilder"/> instance for fluent chaining.</returns>
    /// <example>
    /// <code>
    /// builder
    ///     .Background(RTBColor.FromCss("hsl(210 100% 40% / 50%)"));
    /// </code>
    /// </example>
    public static StyleBuilder Background(this StyleBuilder builder, RTBColor? color)
    {
        return builder.SetIfNotNull("background-color", color);
    }
}
