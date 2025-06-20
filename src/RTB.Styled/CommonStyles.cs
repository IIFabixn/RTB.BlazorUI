using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Styled
{
    [StaticStyle]
    public class CommonStyles
    {
        public static StyleBuilder FullHeight => StyleBuilder.Start.Append("height", "100%");
        public static StyleBuilder FullWidth => StyleBuilder.Start.Append("width", "100%");
    }
}
