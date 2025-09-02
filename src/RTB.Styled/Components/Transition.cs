using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Shorthand-first transitions with typed times and multi-item support.
    /// </summary>
    public class Transition : RTBStyleBase
    {
        /// <summary>When set, emits "transition: …, …" from these items.</summary>
        [Parameter] public IEnumerable<TransitionItem>? Items { get; set; }

        /// <summary>Optional global "will-change: …" hint.</summary>
        [Parameter] public string? WillChange { get; set; }

        /// <summary>
        /// Convenience single transition (emits shorthand). Ignored if <see cref="Items"/> is supplied.
        /// </summary>
        [Parameter] public string Property { get; set; } = "all";

        /// <summary>
        /// E.g. TimeSpan.FromSeconds(0.3) or TimeSpan.FromMilliseconds(150). Default is 0 (no transition).
        /// </summary>
        [Parameter] public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// E.g. TimeSpan.FromSeconds(0.3) or TimeSpan.FromMilliseconds(150). Default is 0 (no delay).
        /// </summary>
        [Parameter] public TimeSpan Delay { get; set; } = TimeSpan.Zero;
        
        /// <summary>E.g. "ease", "linear", "ease-in-out", "cubic-bezier(...)", "steps(...)"</summary>
        [Parameter] public string TimingFunction { get; set; } = "ease";
        
        /// <summary>Optional: "normal" | "allow-discrete" (Transitions Level 2). Single value or comma list.</summary>
        [Parameter] public string? Behavior { get; set; }

        /// <summary>
        /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildStyle(StyleBuilder builder)
        {
            builder.SetIfNotNull("will-change", WillChange);

            var items = Items?.Where(i => i is not null).ToArray();
            if (items is { Length: > 0 })
            {
                var parts = items.Select(it =>
                    $"{(string.IsNullOrWhiteSpace(it.Property) ? "all" : it.Property)} {CssTime(it.Duration)} {(!string.IsNullOrWhiteSpace(it.TimingFunction) ? it.TimingFunction : "ease")} {CssTime(it.Delay)}");

                builder.Set("transition", string.Join(", ", parts));
                builder.SetIfNotNull("transition-behavior", string.Join(", ", items.Select(i => i.Behavior).Where(b => !string.IsNullOrWhiteSpace(b))));
                return;
            }

            // single shorthand
            builder.Set("transition", $"{(string.IsNullOrWhiteSpace(Property) ? "all" : Property)} {CssTime(Duration)} {TimingFunction} {CssTime(Delay)}");
            builder.SetIfNotNull("transition-behavior", Behavior);
        }

        private static string CssTime(TimeSpan t)
        {
            if (t.TotalSeconds < 1) return $"{Math.Round(t.TotalMilliseconds)}ms";
            var s = t.TotalSeconds;
            return s % 1 == 0 ? $"{(int)s}s" : $"{s.ToString("0.###", CultureInfo.InvariantCulture)}s";
        }
    }

    /// <summary>
    /// Single transition item for use in <see cref="Transition.Items"/>.
    /// </summary>
    public sealed class TransitionItem
    {
        /// <summary>
        /// E.g. "all", "opacity", "transform", "background-color", etc. Default is "all".
        /// </summary>
        public string Property { get; set; } = "all";

        /// <summary>
        /// E.g. TimeSpan.FromSeconds(0.3) or TimeSpan.FromMilliseconds(150). Default is 0 (no transition).
        /// </summary>
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// E.g. TimeSpan.FromSeconds(0.3) or TimeSpan.FromMilliseconds(150). Default is 0 (no delay).
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// E.g. "ease", "linear", "ease-in-out", "cubic-bezier(...)", "steps(...)". Default is "ease".
        /// </summary>
        public string TimingFunction { get; set; } = "ease";

        /// <summary>
        /// Optional: "normal" | "allow-discrete" (Transitions Level 2). Single value.
        /// </summary>
        public string? Behavior { get; set; }
    }

    /// <summary>
    /// Style builder extensions for transitions.
    /// </summary>
    public static class TransitionExtensions
    {
        /// <summary>
        /// Sets a transition on an element, with support for multiple items.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static StyleBuilder Transition(this StyleBuilder b, params TransitionItem[] items)
        {
            if (items == null || items.Length == 0) return b;
            var parts = items.Select(i => $"{(string.IsNullOrWhiteSpace(i.Property) ? "all" : i.Property)} {CssTime(i.Duration)} {(!string.IsNullOrWhiteSpace(i.TimingFunction) ? i.TimingFunction : "ease")} {CssTime(i.Delay)}");
            return b.Set("transition", string.Join(", ", parts));
        }

        /// <summary>
        /// Sets a single transition on an element, using shorthand properties.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StyleBuilder WillChange(this StyleBuilder b, string value)
            => b.Set("will-change", value);

        private static string CssTime(TimeSpan t)
        {
            if (t.TotalSeconds < 1) return $"{Math.Round(t.TotalMilliseconds)}ms";
            var s = t.TotalSeconds;
            return s % 1 == 0 ? $"{(int)s}s" : $"{s.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)}s";
        }
    }
}
