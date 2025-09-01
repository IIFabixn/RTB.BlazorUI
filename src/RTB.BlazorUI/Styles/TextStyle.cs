using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using static RTB.Blazor.Styled.Components.Flex;

namespace RTB.Blazor.Styles
{
    public class TextStyle : IStyle
    {
        public string? FontSize { get; set; }
        public string? FontWeight { get; set; }
        public string? LineHeight { get; set; }
        public RTBColor? Color { get; set; }
        public string? TextDecoration { get; set; }
        public Align? TextAlign { get; set; }

        public TextStyle WithColor(RTBColor? color)
        {
            Color = color;
            return this;
        }

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
