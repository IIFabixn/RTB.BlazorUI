using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.Theme.Styles;
using RTB.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class ButtonStyle : TextStyle
    {
        public RTBColor? DisabledColor { get; set; }
    }
}