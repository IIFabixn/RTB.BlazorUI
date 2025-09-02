using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes CSS size-related declarations (width/height and their min/max variants)
/// to the current <see cref="StyleBuilder"/>.
/// </summary>
/// <remarks>
/// - If <see cref="FullWidth"/> is true, a base declaration <c>width: 100%</c> is emitted.
///   If <see cref="Width"/> is also provided, it will override the earlier 100% declaration,
///   because it is applied afterward.
/// - The same precedence applies to <see cref="FullHeight"/> and <see cref="Height"/>.
/// - Use <see cref="SizeExpression"/> values to ensure valid CSS (e.g., px, rem, %, vw, vh).
/// </remarks>
/// <example>
/// The component can be used inside a context where a <see cref="StyleBuilder"/> is cascading:
/// <code>
/// &lt;!-- In a Razor file where StyleBuilder is provided --&gt;
/// &lt;Size FullWidth="true" MinHeight="someSizeExpression" /&gt;
/// </code>
/// Or programmatically through the extension methods:
/// <code>
/// builder.Width(value: widthExpr, min: minWidthExpr, max: maxWidthExpr)
///        .Height(value: heightExpr, min: minHeightExpr, max: maxHeightExpr);
/// </code>
/// </example>
public class Size : RTBStyleBase
{
    /// <summary>
    /// The preferred width as a <see cref="SizeExpression"/> (e.g., 100px, 10rem, 50%).
    /// </summary>
    [Parameter] public SizeExpression? Width { get; set; }

    /// <summary>
    /// The preferred height as a <see cref="SizeExpression"/>.
    /// </summary>
    [Parameter] public SizeExpression? Height { get; set; }

    /// <summary>
    /// The minimum width as a <see cref="SizeExpression"/>.
    /// </summary>
    [Parameter] public SizeExpression? MinWidth { get; set; }

    /// <summary>
    /// The minimum height as a <see cref="SizeExpression"/>.
    /// </summary>
    [Parameter] public SizeExpression? MinHeight { get; set; }

    /// <summary>
    /// The maximum width as a <see cref="SizeExpression"/>.
    /// </summary>
    [Parameter] public SizeExpression? MaxWidth { get; set; }

    /// <summary>
    /// The maximum height as a <see cref="SizeExpression"/>.
    /// </summary>
    [Parameter] public SizeExpression? MaxHeight { get; set; }

    /// <summary>
    /// If true, emits <c>width: 100%</c> before applying <see cref="Width"/>.
    /// If <see cref="Width"/> is set, it will override this declaration.
    /// </summary>
    [Parameter] public bool FullWidth { get; set; }

    /// <summary>
    /// If true, emits <c>height: 100%</c> before applying <see cref="Height"/>.
    /// If <see cref="Height"/> is set, it will override this declaration.
    /// </summary>
    [Parameter] public bool FullHeight { get; set; }

    /// <summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// Applies size-related CSS declarations to the provided <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="StyleBuilder"/> to receive the CSS declarations.
    /// </param>
    /// <remarks>
    /// Order of application:
    /// 1) If <see cref="FullWidth"/> is true, set <c>width: 100%</c>.
    /// 2) Apply <see cref="Width"/>, <see cref="MinWidth"/>, <see cref="MaxWidth"/>.
    /// 3) If <see cref="FullHeight"/> is true, set <c>height: 100%</c>.
    /// 4) Apply <see cref="Height"/>, <see cref="MinHeight"/>, <see cref="MaxHeight"/>.
    /// Later declarations override earlier ones for the same property.
    /// </remarks>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.SetIf("width", "100%", FullWidth);
        builder.Width(Width, MinWidth, MaxWidth);

        builder.SetIf("height", "100%", FullHeight);
        builder.Height(Height, MinHeight, MaxHeight);
    }
}

/// <summary>
/// Extension methods for <see cref="StyleBuilder"/> to compose size-related declarations.
/// </summary>
public static class SizeExtensions
{
    /// <summary>
    /// Set height-related declarations on the <see cref="StyleBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder that receives the declarations.</param>
    /// <param name="value">Optional height value (maps to <c>height</c>).</param>
    /// <param name="min">Optional minimum height (maps to <c>min-height</c>).</param>
    /// <param name="max">Optional maximum height (maps to <c>max-height</c>).</param>
    /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
    public static StyleBuilder Height(this StyleBuilder builder, SizeExpression? value = null, SizeExpression? min = null, SizeExpression? max = null)
    {
        return builder
            .SetIfNotNull("height", value)
            .SetIfNotNull("min-height", min)
            .SetIfNotNull("max-height", max);
    }

    /// <summary>
    /// Set width-related declarations on the <see cref="StyleBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder that receives the declarations.</param>
    /// <param name="value">Optional width value (maps to <c>width</c>).</param>
    /// <param name="min">Optional minimum width (maps to <c>min-width</c>).</param>
    /// <param name="max">Optional maximum width (maps to <c>max-width</c>).</param>
    /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
    public static StyleBuilder Width(this StyleBuilder builder, SizeExpression? value = null, SizeExpression? min = null, SizeExpression? max = null)
    {
        return builder
            .SetIfNotNull("width", value)
            .SetIfNotNull("min-width", min)
            .SetIfNotNull("max-width", max);
    }
}
