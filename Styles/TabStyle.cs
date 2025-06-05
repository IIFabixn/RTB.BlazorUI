using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles
{
    public class TabStyle : IStyle
    {
        public string? Color { get; set; }

        public StyleBuilder ToStyle()
        {
            return StyleBuilder.Start.Append("color", Color);
        }
    }
}
