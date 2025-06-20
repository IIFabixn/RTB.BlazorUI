﻿using RTB.Styled;
using RTB.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class TabStyle : IStyle
    {
        public RTBColor? Color { get; set; }

        public StyleBuilder ToStyle()
        {
            return StyleBuilder.Start.AppendIfNotNull("color", Color);
        }
    }
}
