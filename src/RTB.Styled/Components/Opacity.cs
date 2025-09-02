using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using System.Globalization;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Contributes the CSS <c>opacity</c> declaration to the current <see cref="StyleBuilder"/>.
    /// </summary>
    /// <remarks>
    /// - Accepts values in the range <c>[0, 1]</c> and clamps out-of-range inputs to this interval.<br/>
    /// - When <see cref="Value"/> is <c>null</c>, <see cref="double.NaN"/>, or <see cref="double.IsInfinity(double)"/> is true,
    ///   no declaration is emitted.<br/>
    /// - Uses invariant culture formatting with up to three fractional digits.<br/>
    /// Participation in style composition, registration, and conditional contribution is managed by <see cref="RTBStyleBase"/>.
    /// </remarks>
    /// <example>
    /// Basic usage in a styled scope:
    /// <code>
    /// &lt;Opacity Value="0.5" /&gt;
    /// </code>
    /// Programmatic usage via <see cref="OpacityExtensions.Opacity(StyleBuilder, double?)"/>:
    /// <code>
    /// builder.Opacity(0.75);
    /// </code>
    /// </example>
    public class Opacity : RTBStyleBase
    {
        /// <summary>
        /// The desired opacity value in the range <c>[0, 1]</c>.
        /// </summary>
        /// <remarks>
        /// Values are clamped to <c>[0, 1]</c>. When <c>null</c>, no declaration is emitted.
        /// <br/>Non-finite values (<see cref="double.NaN"/> or infinity) are ignored.
        /// </remarks>
        [Parameter] public double? Value { get; set; }

        /// <summary>
        /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
        /// </summary>
        /// <param name="builder">The builder receiving the <c>opacity</c> declaration.</param>
        /// <remarks>
        /// Emits <c>opacity: &lt;value&gt;</c> using invariant culture with up to three fractional digits when the value is valid.
        /// </remarks>
        protected override void BuildStyle(StyleBuilder builder)
        {
            if (Value is null) return;
            var v = Value.Value;
            if (double.IsNaN(v) || double.IsInfinity(v)) return;
            v = Math.Clamp(v, 0, 1);

            var s = v.ToString("0.###", CultureInfo.InvariantCulture);
            builder.Set("opacity", s);
        }
    }

    /// <summary>
    /// Extension helpers for configuring <c>opacity</c> on a <see cref="StyleBuilder"/>.
    /// </summary>
    public static class OpacityExtensions
    {
        /// <summary>
        /// Sets the CSS <c>opacity</c> declaration on the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The style builder to receive the declaration.</param>
        /// <param name="value">
        /// The desired opacity in the range <c>[0, 1]</c>. Values are clamped.
        /// When <c>null</c>, the builder is returned unchanged.
        /// </param>
        /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
        /// <remarks>
        /// Uses invariant culture with up to three fractional digits.
        /// </remarks>
        public static StyleBuilder Opacity(this StyleBuilder builder, double? value)
        {
            if (value is null) return builder;
            var v = Math.Clamp(value.Value, 0, 1);
            var s = v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
            return builder.Set("opacity", s);
        }
    }
}
