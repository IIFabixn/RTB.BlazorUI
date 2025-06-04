using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Helper;
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

        public virtual StyleBuilder ToStyle(StyleBuilder builder)
        {
            return builder
                .AppendIfNotEmpty("font-size", FontSize)
                .AppendIfNotEmpty("font-weight", FontWeight)
                .AppendIfNotEmpty("line-height", LineHeight)
                .AppendIfNotEmpty("color", Color)
                .AppendIfNotEmpty("text-decoration", TextDecoration);
        }
    }
}
