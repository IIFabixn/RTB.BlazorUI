using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class CardStyle : RTBStyle
    {
        public string CorderRadius { get; set; } = "0.5rem";
        public RTBSpacing HeaderPadding { get; set; } = "0.5rem";
        public string? HeaderBorder { get; set; }
    }
}
