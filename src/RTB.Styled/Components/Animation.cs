using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Extensions;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Describes animation-related CSS longhands and establishes an @keyframes scope for a single animation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// - Non-visual: contributes CSS via a cascading <see cref="StyleBuilder"/>; emits no visible DOM.<br/>
    /// - <see cref="Name"/> is used for both <c>animation-name</c> and as the identifier of the associated <c>@keyframes</c> block.<br/>
    /// - <see cref="Name"/> is cascaded so nested keyframe components can attach frames to the correct <c>@keyframes</c> block.<br/>
    /// - Only animation longhand properties are emitted; the <c>animation</c> shorthand is never used.
    /// </para>
    /// <para>
    /// Usage:
    /// <code>
    /// &lt;Animation Name="fade"
    ///            Duration="@(TimeSpan.FromMilliseconds(250))"
    ///            TimingFunction="ease-out"
    ///            KeyFrames="..."/&gt;
    /// </code>
    /// </para>
    /// </remarks>
    public class Animation : RTBStyleBase
    {
        /// <summary>
        /// Maps to CSS <c>animation-direction</c> keywords.
        /// </summary>
        /// <remarks>
        /// Normal → <c>normal</c>, Reverse → <c>reverse</c>, Alternate → <c>alternate</c>, AlternateReverse → <c>alternate-reverse</c>.
        /// When omitted, the UA default is <c>normal</c>.
        /// </remarks>
        public enum AnimationDirection
        {
            /// <summary>CSS: <c>normal</c>. Plays from 0% to 100% on each cycle.</summary>
            Normal,
            /// <summary>CSS: <c>reverse</c>. Plays from 100% to 0% on each cycle.</summary>
            Reverse,
            /// <summary>CSS: <c>alternate</c>. Even cycles 0%→100%, odd cycles 100%→0%.</summary>
            Alternate,
            /// <summary>CSS: <c>alternate-reverse</c>. Even cycles 100%→0%, odd cycles 0%→100%.</summary>
            AlternateReverse
        }

        /// <summary>
        /// Maps to CSS <c>animation-fill-mode</c> keywords.
        /// </summary>
        /// <remarks>
        /// None → <c>none</c>, Forwards → <c>forwards</c>, Backwards → <c>backwards</c>, Both → <c>both</c>.<br/>
        /// When omitted, the UA default is <c>none</c>.
        /// </remarks>
        public enum AnimationFillMode
        {
            /// <summary>CSS: <c>none</c>. Do not retain styles before/after execution.</summary>
            None,
            /// <summary>CSS: <c>forwards</c>. Retain styles from the last keyframe after completion.</summary>
            Forwards,
            /// <summary>CSS: <c>backwards</c>. Apply styles from the first keyframe during the delay.</summary>
            Backwards,
            /// <summary>CSS: <c>both</c>. Applies both <c>forwards</c> and <c>backwards</c> behaviors.</summary>
            Both
        }

        /// <summary>
        /// Maps to CSS <c>animation-play-state</c> keywords.
        /// </summary>
        /// <remarks>
        /// Running → <c>running</c>, Paused → <c>paused</c>.<br/>
        /// When omitted, the UA default is <c>running</c>.
        /// </remarks>
        public enum AnimationPlayState
        {
            /// <summary>CSS: <c>running</c>. The animation is playing.</summary>
            Running,
            /// <summary>CSS: <c>paused</c>. The animation is paused.</summary>
            Paused
        }

        /// <summary>
        /// The animation name (CSS identifier).
        /// </summary>
        /// <remarks>
        /// - Used for <c>animation-name</c> and as the identifier of the <c>@keyframes</c> block.<br/>
        /// - Must be a valid CSS identifier; no validation or escaping is performed.
        /// </remarks>
        [Parameter, EditorRequired] public required string Name { get; set; }

        // animation-* properties

        /// <summary>
        /// Maps to <c>animation-duration</c>. Omitted when <c>null</c>.
        /// </summary>
        /// <remarks>
        /// - Uses seconds (s) for values ≥ 1s (up to 3 decimals) and milliseconds (ms) for values &lt; 1s (rounded to nearest ms).<br/>
        /// - Examples: 250ms → "250ms", 1.5s → "1.5s", 2s → "2s".<br/>
        /// - When omitted, the UA default is <c>0s</c>, which makes the animation instantaneous.
        /// </remarks>
        [Parameter] public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Maps to <c>animation-timing-function</c> (e.g., <c>ease</c>, <c>linear</c>, <c>ease-in-out</c>, <c>cubic-bezier(...)</c>, <c>steps(...)</c>).
        /// </summary>
        /// <remarks>
        /// Omitted when null or whitespace. No validation is performed. When omitted, the UA default is <c>ease</c>.
        /// </remarks>
        [Parameter] public string? TimingFunction { get; set; }

        /// <summary>
        /// Maps to <c>animation-delay</c>. Omitted when <c>null</c>.
        /// </summary>
        /// <remarks>
        /// - Uses the same formatting rules as <see cref="Duration"/>.<br/>
        /// - Examples: 75ms → "75ms", 2s → "2s".<br/>
        /// - When omitted, the UA default is <c>0s</c>.
        /// </remarks>
        [Parameter] public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Maps to <c>animation-iteration-count</c> when <see cref="Infinite"/> is false.
        /// </summary>
        /// <remarks>
        /// - Ignored when <see cref="Infinite"/> is true (then <c>infinite</c> is emitted).<br/>
        /// - Not validated; callers should avoid negative values per CSS spec.<br/>
        /// - When both this and <see cref="Infinite"/> are omitted/false, the UA default is <c>1</c>.
        /// </remarks>
        [Parameter] public int? IterationCount { get; set; }

        /// <summary>
        /// If true, sets <c>animation-iteration-count: infinite</c> and ignores <see cref="IterationCount"/>.
        /// </summary>
        [Parameter] public bool Infinite { get; set; }

        /// <summary>
        /// Maps to <c>animation-direction</c>. Omitted when <c>null</c>.
        /// </summary>
        [Parameter] public AnimationDirection? Direction { get; set; }

        /// <summary>
        /// Maps to <c>animation-fill-mode</c>. Omitted when <c>null</c>.
        /// </summary>
        [Parameter] public AnimationFillMode? FillMode { get; set; }

        /// <summary>
        /// Maps to <c>animation-play-state</c>. Omitted when <c>null</c>.
        /// </summary>
        [Parameter] public AnimationPlayState? PlayState { get; set; }

        /// <summary>
        /// Maps to <c>animation-composition</c> (CSS Animations Level 2).
        /// </summary>
        /// <remarks>
        /// - Typical values include <c>replace</c> (default in most engines) or <c>add</c>.<br/>
        /// - Browser support may be limited/experimental; value is emitted as-is without validation.<br/>
        /// - Omitted when null or whitespace.
        /// </remarks>
        [Parameter] public string? Composition { get; set; } // optional: "replace" | "add" (Animations 2)

        /// <summary>
        /// Child content that defines frames for the <c>@keyframes</c> named by <see cref="Name"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Name"/> is cascaded so child keyframe components attach frames to the correct block.
        /// </remarks>
        [Parameter, EditorRequired] public required RenderFragment KeyFrames { get; set; }

        /// <summary>
        /// Contributes animation-related CSS declarations to the current <see cref="StyleBuilder"/>.
        /// </summary>
        /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
        protected override void BuildStyle(StyleBuilder builder)
        {
            builder
                .AnimationName(Name)
                .AnimationDuration(Duration)
                .AnimationTiming(TimingFunction)
                .AnimationDelay(Delay)
                .AnimationIterationCount(IterationCount, Infinite)
                .AnimationDirection(Direction)
                .AnimationFillMode(FillMode)
                .AnimationPlayState(PlayState)
                .SetIfNotNull("animation-composition", Composition);
        }

        /// <summary>
        /// Emits a cascading value containing <see cref="Name"/> so nested keyframe components
        /// can bind to the corresponding <c>@keyframes</c> block.
        /// </summary>
        /// <param name="builder">The render tree builder.</param>
        /// <remarks>
        /// - Produces no visual markup; this component affects only style generation and keyframes scope.<br/>
        /// - The cascading parameter name is <c>Keyframe.AnimationName</c> to match keyframe components' expectations.
        /// </remarks>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<string>>(0);
            builder.AddAttribute(1, "Value", Name);
            builder.AddAttribute(2, "Name", nameof(Keyframe.AnimationName));
            builder.AddAttribute(3, "ChildContent", KeyFrames);
            builder.CloseComponent();
        }
    }

    /// <summary>
    /// Extension helpers that map <see cref="Animation"/> parameters to CSS <c>animation-*</c> longhand declarations.
    /// </summary>
    /// <remarks>
    /// - These helpers set individual longhand properties; no shorthand <c>animation</c> is emitted.<br/>
    /// - Methods omit properties when inputs are null/whitespace; no validation of CSS values is performed.<br/>
    /// - Returned <see cref="StyleBuilder"/> enables fluent chaining.
    /// </remarks>
    public static class AnimationStyleExtensions
    {
        /// <summary>
        /// Sets <c>animation-name</c> when <paramref name="name"/> is not null/whitespace; otherwise omitted.
        /// </summary>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationName(this StyleBuilder b, string? name)
            => b.SetIfNotNull("animation-name", name);

        /// <summary>
        /// Sets <c>animation-duration</c> using s/ms formatting.
        /// </summary>
        /// <remarks>
        /// - When <paramref name="t"/> is null, an empty value is passed which is ignored by the underlying declaration set.<br/>
        /// - Values &lt; 1 second are rounded to the nearest millisecond (e.g., 249.6ms → "250ms").<br/>
        /// - Values ≥ 1 second use seconds; integers are emitted without decimals (e.g., "2s"), fractional values use up to 3 decimals (e.g., "1.25s").
        /// </remarks>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationDuration(this StyleBuilder b, TimeSpan? t)
            => b.Set("animation-duration", CssTime(t));

        /// <summary>
        /// Sets <c>animation-delay</c> using s/ms formatting.
        /// </summary>
        /// <remarks>
        /// Uses the same formatting and omission rules as <see cref="AnimationDuration(StyleBuilder, TimeSpan?)"/>.
        /// </remarks>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationDelay(this StyleBuilder b, TimeSpan? t)
            => b.Set("animation-delay", CssTime(t));

        /// <summary>
        /// Sets <c>animation-timing-function</c> when <paramref name="tf"/> is not null/whitespace; otherwise omitted.
        /// </summary>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationTiming(this StyleBuilder b, string? tf)
            => b.SetIfNotNull("animation-timing-function", tf);

        /// <summary>
        /// Sets <c>animation-iteration-count</c> to either a number or <c>infinite</c>.
        /// </summary>
        /// <param name="b">The <see cref="StyleBuilder"/> being configured.</param>
        /// <param name="count">Number of iterations when <paramref name="infinite"/> is false. Not validated; avoid negative values per CSS spec.</param>
        /// <param name="infinite">When true, sets <c>infinite</c> and ignores <paramref name="count"/>.</param>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationIterationCount(this StyleBuilder b, int? count, bool infinite = false)
        {
            if (infinite) return b.Set("animation-iteration-count", "infinite");
            return b.SetIfNotNull("animation-iteration-count", count?.ToString());
        }

        /// <summary>
        /// Sets <c>animation-direction</c> when <paramref name="dir"/> is not null; otherwise omitted.
        /// </summary>
        /// <remarks>
        /// Maps enum values to CSS keywords: Normal → <c>normal</c>, Reverse → <c>reverse</c>, Alternate → <c>alternate</c>, AlternateReverse → <c>alternate-reverse</c>.
        /// </remarks>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationDirection(this StyleBuilder b, Animation.AnimationDirection? dir)
            => b.SetIfNotNull("animation-direction", dir?.ToCss());

        /// <summary>
        /// Sets <c>animation-fill-mode</c> when <paramref name="fill"/> is not null; otherwise omitted.
        /// </summary>
        /// <remarks>
        /// Maps enum values to CSS keywords: None → <c>none</c>, Forwards → <c>forwards</c>, Backwards → <c>backwards</c>, Both → <c>both</c>.
        /// </remarks>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationFillMode(this StyleBuilder b, Animation.AnimationFillMode? fill)
            => b.SetIfNotNull("animation-fill-mode", fill?.ToCss());

        /// <summary>
        /// Sets <c>animation-play-state</c> when <paramref name="st"/> is not null; otherwise omitted.
        /// </summary>
        /// <remarks>
        /// Maps enum values to CSS keywords: Running → <c>running</c>, Paused → <c>paused</c>.
        /// </remarks>
        /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
        public static StyleBuilder AnimationPlayState(this StyleBuilder b, Animation.AnimationPlayState? st)
            => b.SetIfNotNull("animation-play-state", st?.ToCss());

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> for CSS time values.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><description>When <paramref name="t"/> is null, returns an empty string (ignored by the declaration set).</description></item>
        ///   <item><description>For values &lt; 1 second, uses integer milliseconds (rounded): e.g., 249.6ms → "250ms".</description></item>
        ///   <item><description>For values ≥ 1 second, uses seconds:
        ///     integers as "Ns" (e.g., "2s") and fractional seconds with up to 3 decimals (e.g., "1.25s").</description></item>
        /// </list>
        /// </remarks>
        private static string CssTime(TimeSpan? t)
        {
            if (t is null) return string.Empty;
            var ts = t.Value;
            if (ts.TotalSeconds < 1) return $"{Math.Round(ts.TotalMilliseconds)}ms";
            var s = ts.TotalSeconds;
            return s % 1 == 0 ? $"{(int)s}s" : $"{s:0.###}s";
        }
    }
}
