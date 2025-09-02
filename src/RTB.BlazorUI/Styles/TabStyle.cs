using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styles
{
    /// <summary>
    /// Tab style settings.
    /// </summary>
    public class TabStyle : IStyle
    {
        /// <summary>
        /// The color of the tab text.
        /// </summary>
        public RTBColor? Color { get; set; }

        /// <summary>
        /// Builds the style.
        /// </summary>
        /// <returns></returns>
        public StyleBuilder ToStyle()
        {
            return StyleBuilder.Start.SetIfNotNull("color", Color);
        }
    }
}
