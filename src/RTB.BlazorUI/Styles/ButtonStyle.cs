using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styles;

public class ButtonStyle : TextStyle
{
    public RTBColor? DisabledColor { get; set; }
    public RTBColor? BackgroundColor { get; set; }
    public RTBColor? DisabledBackgroundColor { get; set; }
}