using RTB.Styled;
using RTB.Styled.Helper;
using static RTB.Styled.Components.Flex;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class TextStyle : IStyle
    {
        public string? FontSize { get; set; }
        public string? FontWeight { get; set; }
        public string? LineHeight { get; set; }
        public RTBColor? Color { get; set; }
        public string? TextDecoration { get; set; }
        public Align? TextAlign { get; set; }

        public virtual StyleBuilder ToStyle()
        {
            return StyleBuilder.Start
                .AppendIfNotNull("font-size", FontSize)
                .AppendIfNotNull("font-weight", FontWeight)
                .AppendIfNotNull("line-height", LineHeight)
                .AppendIfNotNull("color", Color)
                .AppendIfNotNull("text-decoration", TextDecoration)
                .AppendIfNotNull("text-align", TextAlign?.ToCss());
        }
    }
}
