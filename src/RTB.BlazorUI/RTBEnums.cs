using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor
{
    /// <summary>
    /// Represents the position of a UI element relative to a reference element or container.
    /// </summary>
    /// <remarks>
    /// Commonly used by RTB.Blazor components to control alignment and placement
    /// such as tooltips, popovers, context menus, and flyouts.
    /// </remarks>
    public enum Position
    {
        /// <summary>
        /// Positioned above the reference element.
        /// </summary>
        Top,

        /// <summary>
        /// Positioned to the right of the reference element.
        /// </summary>
        Right,

        /// <summary>
        /// Positioned below the reference element.
        /// </summary>
        Bottom,

        /// <summary>
        /// Positioned to the left of the reference element.
        /// </summary>
        Left
    }
}
