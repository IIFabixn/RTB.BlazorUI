using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper
{
    /// <summary>
    /// Represents a responsive CSS media query breakpoint.
    /// </summary>
    /// <remarks>
    /// - Builds a media query string based on <see cref="Media"/>, <see cref="MinWidth"/>, and <see cref="MaxWidth"/>.
    /// - The generated string intentionally omits the leading "@media " keyword.
    /// - <see cref="Orientation"/> is currently not emitted by <see cref="ToQuery"/> and is reserved for future use.
    /// - <see cref="SizeExpression"/> instances render to valid CSS when converted to string; operations compose into CSS calc().
    /// </remarks>
    public class BreakPoint
    {
        /// <summary>
        /// Supported CSS media types.
        /// </summary>
        public enum MediaType
        {
            /// <summary>
            /// All devices.
            /// </summary>
            All,
            /// <summary>
            /// Screens (default).
            /// </summary>
            Screen,
            /// <summary>
            /// Print devices.
            /// </summary>
            Print
        }

        /// <summary>
        /// Supported device orientation values.
        /// </summary>
        /// <remarks>
        /// Not currently included in the output of <see cref="ToQuery"/>; reserved for future support.
        /// </remarks>
        public enum OrientationType
        {
            /// <summary>
            /// Portrait orientation.
            /// </summary>
            Portrait,
            /// <summary>
            /// Landscape orientation.
            /// </summary>
            Landscape
        }

        /// <summary>
        /// The media type to target. Defaults to <see cref="MediaType.Screen"/>.
        /// </summary>
        /// <remarks>
        /// Emitted as a lower-cased token in <see cref="ToQuery"/> via an extension like <c>ToCss()</c>.
        /// </remarks>
        public MediaType Media { get; set; } = MediaType.Screen;

        /// <summary>
        /// Optional orientation constraint.
        /// </summary>
        /// <remarks>
        /// Currently not emitted by <see cref="ToQuery"/>. Set for completeness or future extension.
        /// </remarks>
        public OrientationType? Orientation { get; set; } = null;

        /// <summary>
        /// Optional minimum width constraint for the media query, e.g., <c>(min-width: 768px)</c>.
        /// </summary>
        /// <remarks>
        /// Provide a <see cref="SizeExpression"/>. Its string representation becomes the CSS value.
        /// </remarks>
        public SizeExpression? MinWidth { get; set; }

        /// <summary>
        /// Optional maximum width constraint for the media query, e.g., <c>(max-width: 1200px)</c>.
        /// </summary>
        /// <remarks>
        /// Provide a <see cref="SizeExpression"/>. Its string representation becomes the CSS value.
        /// </remarks>
        public SizeExpression? MaxWidth { get; set; }

        /// <summary>
        /// Builds the media query condition string.
        /// </summary>
        /// <returns>
        /// A CSS media condition string without the leading "@media " prefix.
        /// Example: <c>screen and (min-width: 768px) and (max-width: 1200px)</c>.
        /// </returns>
        /// <remarks>
        /// - The media token is lower-cased.
        /// - Includes <see cref="MinWidth"/> and/or <see cref="MaxWidth"/> when provided.
        /// - Does not include <see cref="Orientation"/> at this time.
        /// </remarks>
        public string ToQuery()
        {
            var query = new StringBuilder();
            query.Append(Media.ToCss().ToLowerInvariant());

            if (MinWidth is not null)
            {
                query.Append($" and (min-width: {MinWidth})");
            }

            if (MaxWidth is not null)
            {
                query.Append($" and (max-width: {MaxWidth})");
            }

            return query.ToString();
        }
    }
}
