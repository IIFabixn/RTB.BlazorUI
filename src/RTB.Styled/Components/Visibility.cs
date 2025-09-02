using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Sets the visibility of an element.
    /// </summary>
    public class Visibility : RTBStyleBase
    {
        /// <summary>
        /// The visibility mode.
        /// </summary>
        public enum Mode 
        {
            /// <summary>
            /// The element is visible.
            /// </summary>
            Visible,

            /// <summary>
            /// The element is hidden, but still takes up space.
            /// </summary>
            Hidden,

            /// <summary>
            /// The element is hidden and does not take up space.
            /// </summary>
            Collapse
        }

        /// <summary>
        /// The visibility mode. Default is <see cref="Mode.Visible"/>.
        /// </summary>
        [Parameter] public Mode Value { get; set; } = Mode.Visible;

        /// <summary>
        /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildStyle(StyleBuilder builder)
        {
            builder.Visibility(Value);
        }
    }

    /// <summary>
    /// Style builder extensions for visibility.
    /// </summary>
    public static class VisibilityExtensions
    {
        /// <summary>
        /// Sets the visibility of an element.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StyleBuilder Visibility(this StyleBuilder builder, Visibility.Mode value)
        {
            return builder.Set("visibility", value.ToCss());
        }
    }
}
