using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes CSS margin declarations to the current <see cref="StyleBuilder"/> scope.
/// </summary>
/// <remarks>
/// Precedence (later entries override earlier ones):
/// 1) <see cref="All"/> sets the "margin" shorthand for all sides.
/// 2) <see cref="Horizontal"/>/<see cref="Vertical"/> set the "margin" shorthand as "<c>&lt;vertical-or-0&gt; &lt;horizontal-or-0&gt;</c>" (top/bottom, left/right).
///    If only one of the two is provided, the missing counterpart defaults to <c>0</c>.
/// 3) Side-specific properties (<see cref="Top"/>, <see cref="Right"/>, <see cref="Bottom"/>, <see cref="Left"/>) override any previous shorthand.
/// Null values are ignored.
/// </remarks>
/// <example>
/// Usage in a component that provides a cascading StyleBuilder:
/// <code>
/// &lt;Margin All="Spacing.Rem(1)" /&gt;
/// &lt;Margin Horizontal="Spacing.Px(8)" Vertical="Spacing.Px(12)" /&gt;
/// &lt;Margin Vertical="Spacing.Px(12)" Top="Spacing.Px(4)" /&gt;
/// </code>
/// </example>
public class Margin : RTBStyleBase
{
    /// <summary>
    /// Shorthand to set the same margin on all four sides (emits <c>margin: ...</c>).
    /// Overridden by <see cref="Horizontal"/>, <see cref="Vertical"/>, and side-specific properties when provided.
    /// </summary>
    [Parameter] public Spacing? All { get; set; }

    /// <summary>
    /// Sets the top margin (emits <c>margin-top</c>). Overrides any previously emitted shorthand.
    /// </summary>
    [Parameter] public Spacing? Top { get; set; }

    /// <summary>
    /// Sets the right margin (emits <c>margin-right</c>). Overrides any previously emitted shorthand.
    /// </summary>
    [Parameter] public Spacing? Right { get; set; }

    /// <summary>
    /// Sets the bottom margin (emits <c>margin-bottom</c>). Overrides any previously emitted shorthand.
    /// </summary>
    [Parameter] public Spacing? Bottom { get; set; }

    /// <summary>
    /// Sets the left margin (emits <c>margin-left</c>). Overrides any previously emitted shorthand.
    /// </summary>
    [Parameter] public Spacing? Left { get; set; }

    /// <summary>
    /// Shorthand for left/right margins. When <see cref="Horizontal"/> or <see cref="Vertical"/> is set,
    /// emits <c>margin: &lt;vertical-or-0&gt; &lt;horizontal-or-0&gt;</c>.
    /// </summary>
    [Parameter] public Spacing? Horizontal { get; set; }

    /// <summary>
    /// Shorthand for top/bottom margins. When <see cref="Horizontal"/> or <see cref="Vertical"/> is set,
    /// emits <c>margin: &lt;vertical-or-0&gt; &lt;horizontal-or-0&gt;</c>.
    /// </summary>
    [Parameter] public Spacing? Vertical { get; set; }

    /// <summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// </summary>
    /// <remarks>
    /// Emission order ensures the precedence described in the class remarks:
    /// 1) <see cref="All"/>, 2) <see cref="Horizontal"/>/<see cref="Vertical"/>, 3) side-specific properties.
    /// </remarks>
    /// <param name="builder">The style builder receiving declarations.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.SetIfNotNull("margin", All);

        // If either Horizontal or Vertical is provided, emit a "margin: <vertical> <horizontal>" shorthand.
        // Missing counterpart defaults to 0.
        builder.SetIf("margin", $"{Vertical ?? 0} {Horizontal ?? 0}", Horizontal is not null || Vertical is not null);

        // Side-specific overrides
        builder.SetIfNotNull("margin-top", Top);
        builder.SetIfNotNull("margin-right", Right);
        builder.SetIfNotNull("margin-bottom", Bottom);
        builder.SetIfNotNull("margin-left", Left);
    }
}

/// <summary>
/// Fluent extensions for applying CSS margin declarations using <see cref="StyleBuilder"/>.
/// </summary>
public static class MarginExtensions
{
    /// <summary>
    /// Applies margin declarations to the builder.
    /// </summary>
    /// <remarks>
    /// Precedence (later entries override earlier ones):
    /// 1) <paramref name="all"/> sets the "margin" shorthand for all sides.
    /// 2) <paramref name="horizontal"/>/<paramref name="vertical"/> set the "margin" shorthand as "<c>&lt;vertical-or-0&gt; &lt;horizontal-or-0&gt;</c>".
    ///    If only one of the two is provided, the missing counterpart defaults to <c>0</c>.
    /// 3) Side-specific parameters override any previous shorthand.
    /// </remarks>
    /// <param name="builder">The target style builder.</param>
    /// <param name="all">Shorthand for all four sides.</param>
    /// <param name="top">Top margin.</param>
    /// <param name="right">Right margin.</param>
    /// <param name="bottom">Bottom margin.</param>
    /// <param name="left">Left margin.</param>
    /// <param name="horizontal">Shorthand for left/right.</param>
    /// <param name="vertical">Shorthand for top/bottom.</param>
    /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
    public static StyleBuilder Margin(this StyleBuilder builder,
        Spacing? all = null,
        Spacing? top = null,
        Spacing? right = null,
        Spacing? bottom = null,
        Spacing? left = null,
        Spacing? horizontal = null,
        Spacing? vertical = null)
    {
        return builder
            .SetIfNotNull("margin", all)
            .SetIf("margin", $"{vertical ?? 0} {horizontal ?? 0}", horizontal is not null || vertical is not null)
            .SetIfNotNull("margin-top", top)
            .SetIfNotNull("margin-right", right)
            .SetIfNotNull("margin-bottom", bottom)
            .SetIfNotNull("margin-left", left);
    }
}
