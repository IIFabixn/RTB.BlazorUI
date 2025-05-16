using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Themes
{
    [Theme(IsDefault = true)]
    public class RTBLightTheme : RTBBaseTheme
    {
        public override string Paper => "#FFFFFF";
        public override string Shadow => "0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)";
    }
}
