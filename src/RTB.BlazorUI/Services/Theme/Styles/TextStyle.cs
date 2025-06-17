using Microsoft.AspNetCore.Components;
using RTB.Styled;
using RTB.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class TextStyle : IStyle
    {
        public string? FontSize { get; set; }
        public string? FontWeight { get; set; }
        public string? LineHeight { get; set; }
        public RTBColor? Color { get; set; }
        public string? TextDecoration { get; set; }

        public virtual StyleBuilder ToStyle()
        {
            return StyleBuilder.Start
                .AppendIfNotNull("font-size", FontSize)
                .AppendIfNotNull("font-weight", FontWeight)
                .AppendIfNotNull("line-height", LineHeight)
                .AppendIfNotNull("color", Color)
                .AppendIfNotNull("text-decoration", TextDecoration);
        }
    }
}
