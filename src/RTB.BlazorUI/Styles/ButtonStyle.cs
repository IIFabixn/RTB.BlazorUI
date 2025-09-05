using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styles;

/// <summary>
/// Style model for buttons, extending <see cref="TextStyle"/> with button-specific colors.
/// </summary>
/// <remarks>
/// This type is a passive data model: it does not emit CSS by itself. Consumers (e.g., Blazor components)
/// should translate its properties into CSS using <see cref="RTB.Blazor.Styled.Core.StyleBuilder"/> or similar mechanisms.
/// A <c>null</c> property indicates that no explicit value should be emitted and theming/defaults should apply.
/// </remarks>
/// <example>
/// Example of using ButtonStyle with StyleBuilder:
/// <code>
/// var bs = new ButtonStyle
/// {
///     BackgroundColor = RTBColor.FromRgb(33, 150, 243), // blue
///     DisabledBackgroundColor = RTBColor.FromRgb(189, 189, 189), // grey
///     DisabledColor = RTBColor.FromRgb(255, 255, 255) // white
/// }.WithColor(RTBColor.FromRgb(255, 255, 255)); // text color for normal state
///
/// var sb = bs.ToStyle()
///     .SetIf("background-color", bs.BackgroundColor?.HexRgba, bs.BackgroundColor.HasValue)
///     .SetIf("color", bs.Color?.HexRgba, bs.Color.HasValue)
///     .Selector("&amp;:disabled", b => b
///         .SetIf("background-color", bs.DisabledBackgroundColor?.HexRgba, bs.DisabledBackgroundColor.HasValue)
///         .SetIf("color", bs.DisabledColor?.HexRgba, bs.DisabledColor.HasValue));
/// </code>
/// </example>
public class ButtonStyle : TextStyle
{
    /// <summary>
    /// Foreground text color to use when the button is disabled.
    /// </summary>
    /// <remarks>
    /// If <c>null</c>, consumers should avoid emitting an explicit disabled text color and
    /// rely on default styles or theming.
    /// </remarks>
    public RTBColor? DisabledColor { get; set; }

    /// <summary>
    /// Background color for the button in its normal (enabled) state.
    /// </summary>
    /// <remarks>
    /// Use helpers like <see cref="RTBColor.FromRgb(byte, byte, byte)"/> or <see cref="RTBColor.Parse(string)"/>
    /// to construct values.
    /// </remarks>
    public RTBColor? BackgroundColor { get; set; }

    /// <summary>
    /// Background color for the button when it is disabled.
    /// </summary>
    /// <remarks>
    /// If <c>null</c>, consumers may fall back to <see cref="BackgroundColor"/> or theme defaults.
    /// </remarks>
    public RTBColor? DisabledBackgroundColor { get; set; }

    /// <inheritdoc cref="IStyle.ToStyle()"/>
    public override StyleBuilder ToStyle()
    {
        return base.ToStyle()
            .Background(BackgroundColor);
    }
}