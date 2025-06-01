using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class TextStyle : RTBStyle
    {
        public string? FontSize { get; set; }
        public string? FontWeight { get; set; }
        public string? LineHeight { get; set; }
        public RTBColor? Color { get; set; }
        public string? TextDecoration { get; set; }

        public override string ToStyle()
        {
            return StyleBuilder.Create(base.ToStyle())
                .AppendIfNotEmpty("font-size", FontSize)
                .AppendIfNotEmpty("font-weight", FontWeight)
                .AppendIfNotEmpty("line-height", LineHeight)
                .AppendIfNotEmpty("color", Color?.Hex)
                .AppendIfNotEmpty("text-decoration", TextDecoration)
                .Build();
        }
    }
}
