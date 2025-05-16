using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Themes
{
    public abstract class RTBBaseTheme : IRTBTheme
    {
        public abstract string Paper { get; }

        public virtual string CornerRadius => "0.5rem";

        public abstract string Shadow { get; }
    }
}
