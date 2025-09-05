using RTB.Blazor.Styled.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styles
{
    /// <summary>
    /// Style definition specialized for text input / text field components.
    /// </summary>
    /// <remarks>
    /// Inherits all text related properties from <see cref="TextStyle"/> and adds a <see cref="BackgroundColor"/> property.
    /// Use <see cref="ToStyle"/> to convert this instance into a <see cref="StyleBuilder"/> suitable for emission
    /// as scoped CSS in Blazor components.
    /// </remarks>
    /// <example>
    /// <code>
    /// var style = new TextFieldStyle
    /// {
    ///     FontSize = "14px",
    ///     FontWeight = "500",
    ///     BackgroundColor = RTBColor.Parse("#f5f5f5"),
    ///     Color = RTBColor.Parse("#333")
    /// };
    /// var builder = style.ToStyle();
    /// </code>
    /// </example>
    public class TextFieldStyle : TextStyle
    {
        /// <summary>
        /// Optional background color for the text field container / input surface.
        /// When null, no background declaration is emitted (allowing inheritance or external overrides).
        /// </summary>
        public RTBColor? BackgroundColor { get; set; }

        /// <summary>
        /// Creates a <see cref="StyleBuilder"/> with all base text declarations plus
        /// the background color (if provided).
        /// </summary>
        /// <returns>A configured <see cref="StyleBuilder"/>.</returns>
        /// <remarks>
        /// This override calls the base implementation first, then appends a background
        /// declaration only when <see cref="BackgroundColor"/> has a value.
        /// </remarks>
        public override StyleBuilder ToStyle()
        {
            return base.ToStyle()
                .Background(BackgroundColor);
        }
    }
}
