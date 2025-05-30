using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class RTBStyle : IStyle
    {
        public RTBColor? Background { get; set; }
        public RTBSpacing? Padding { get; set; }
        public RTBSpacing? Margin { get; set; }

        public virtual string ToStyle()
        {
            return StyleBuilder.Create()
                .AppendIfNotEmpty("padding", Padding)
                .AppendIfNotEmpty("margin", Margin)
                .AppendIfNotEmpty("background-color", Background?.Hex)
                .Build();
        }
    }
}
