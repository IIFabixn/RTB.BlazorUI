using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using static RTB.Blazor.Styled.Components.Flex;

namespace RTB.Blazor.Styles
{
    /// <summary>
    /// Style class for text-related CSS properties.
    /// </summary>
    public class TextStyle : IStyle
    {
        /// <summary>
        /// CSS font-size property.
        /// </summary>
        public string? FontSize { get; set; }

        /// <summary>
        /// CSS font-weight property.
        /// </summary>
        public string? FontWeight { get; set; }

        /// <summary>
        /// CSS line-height property.
        /// </summary>
        public string? LineHeight { get; set; }

        /// <summary>
        /// CSS color property.
        /// </summary>
        public RTBColor? Color { get; set; }

        /// <summary>
        /// CSS text-decoration property.
        /// </summary>
        public string? TextDecoration { get; set; }

        /// <summary>
        /// CSS text-align property.
        /// </summary>
        public Align? TextAlign { get; set; }

        /// <summary>
        /// Sets the Color property and returns the current instance for chaining.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public TextStyle WithColor(RTBColor? color)
        {
            Color = color;
            return this;
        }

        /// <summary>
        /// Converts the TextStyle properties to a StyleBuilder instance.
        /// </summary>
        /// <returns></returns>
        public virtual StyleBuilder ToStyle()
        {
            return StyleBuilder.Start
                .SetIfNotNull("font-size", FontSize)
                .SetIfNotNull("font-weight", FontWeight)
                .SetIfNotNull("line-height", LineHeight)
                .SetIfNotNull("color", Color)
                .SetIfNotNull("text-decoration", TextDecoration)
                .SetIfNotNull("text-align", TextAlign?.ToCss());
        }
    }
}
