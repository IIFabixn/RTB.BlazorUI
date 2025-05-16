using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Themes
{
    public interface IRTBTheme : ITheme
    {
        string Paper { get; }
        string CornerRadius { get; }
        string Shadow { get; }
    }
}
