using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;

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
        [Parameter] public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        [Parameter] public TimeSpan Delay { get; set; } = TimeSpan.Zero;
        /// <summary>E.g. "ease", "linear", "ease-in-out", "cubic-bezier(...)", "steps(...)"</summary>
        [Parameter] public string TimingFunction { get; set; } = "ease";
        /// <summary>Optional: "normal" | "allow-discrete" (Transitions Level 2). Single value or comma list.</summary>
        [Parameter] public string? Behavior { get; set; }

        public override StyleBuilder BuildStyle(StyleBuilder builder)
        {
            if (!Condition) return builder;

            builder.AppendIfNotNull("will-change", WillChange);

            var items = Items?.Where(i => i is not null).ToArray();
            if (items is { Length: > 0 })
            {
                var parts = items.Select(it =>
                    $"{(string.IsNullOrWhiteSpace(it.Property) ? "all" : it.Property)} {CssTime(it.Duration)} {(!string.IsNullOrWhiteSpace(it.TimingFunction) ? it.TimingFunction : "ease")} {CssTime(it.Delay)}");

                builder.Append("transition", string.Join(", ", parts));
                builder.AppendIfNotNull("transition-behavior", string.Join(", ", items.Select(i => i.Behavior).Where(b => !string.IsNullOrWhiteSpace(b))));
                return builder;
            }

            // single shorthand
            builder.Append("transition", $"{(string.IsNullOrWhiteSpace(Property) ? "all" : Property)} {CssTime(Duration)} {TimingFunction} {CssTime(Delay)}");
            builder.AppendIfNotNull("transition-behavior", Behavior);
            return builder;
        }

        private static string CssTime(TimeSpan t)
        {
            if (t.TotalSeconds < 1) return $"{Math.Round(t.TotalMilliseconds)}ms";
            var s = t.TotalSeconds;
            return s % 1 == 0 ? $"{(int)s}s" : $"{s.ToString("0.###", CultureInfo.InvariantCulture)}s";
        }
    }

    public sealed class TransitionItem
    {
        public string Property { get; set; } = "all";
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;
        public string TimingFunction { get; set; } = "ease";
        public string? Behavior { get; set; }
    }

    public static class TransitionExtensions
    {
        public static StyleBuilder Transition(this StyleBuilder b, params TransitionItem[] items)
        {
            if (items == null || items.Length == 0) return b;
            var parts = items.Select(i => $"{(string.IsNullOrWhiteSpace(i.Property) ? "all" : i.Property)} {CssTime(i.Duration)} {(!string.IsNullOrWhiteSpace(i.TimingFunction) ? i.TimingFunction : "ease")} {CssTime(i.Delay)}");
            return b.Append("transition", string.Join(", ", parts));
        }

        public static StyleBuilder WillChange(this StyleBuilder b, string value)
            => b.Append("will-change", value);

        private static string CssTime(TimeSpan t)
        {
            if (t.TotalSeconds < 1) return $"{Math.Round(t.TotalMilliseconds)}ms";
            var s = t.TotalSeconds;
            return s % 1 == 0 ? $"{(int)s}s" : $"{s.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)}s";
        }
    }
}
