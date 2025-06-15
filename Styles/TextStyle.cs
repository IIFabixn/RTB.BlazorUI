using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles
{
    public class TextStyle : IStyle
    {
        public string? FontSize { get; set; }
        public string? FontWeight { get; set; }
        public string? LineHeight { get; set; }
        public string? Color { get; set; }
        public string? TextDecoration { get; set; }

        public virtual StyleBuilder ToStyle()
        {
            return StyleBuilder.Start
                .Append("font-size", FontSize)
                .Append("font-weight", FontWeight)
                .Append("line-height", LineHeight)
                .Append("color", Color)
                .Append("text-decoration", TextDecoration);
        }
    }
}
