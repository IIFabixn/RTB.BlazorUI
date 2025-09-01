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
    public class Animation : RTBStyleBase
    {
        public enum AnimationDirection { Normal, Reverse, Alternate, AlternateReverse }
        public enum AnimationFillMode { None, Forwards, Backwards, Both }
        public enum AnimationPlayState { Running, Paused }

        [Parameter, EditorRequired] public required string Name { get; set; }

        // animation-* properties
        [Parameter] public TimeSpan? Duration { get; set; }
        [Parameter] public string? TimingFunction { get; set; }
        [Parameter] public TimeSpan? Delay { get; set; }
        [Parameter] public int? IterationCount { get; set; }
        [Parameter] public bool Infinite { get; set; }
        [Parameter] public AnimationDirection? Direction { get; set; }
        [Parameter] public AnimationFillMode? FillMode { get; set; }
        [Parameter] public AnimationPlayState? PlayState { get; set; }
        [Parameter] public string? Composition { get; set; } // optional: "replace" | "add" (Animations 2)

        [Parameter, EditorRequired] public required RenderFragment KeyFrames { get; set; }

        public override IStyleBuilder BuildStyle(IStyleBuilder builder)
        {
            if (!Condition) return builder;

            // 1) Emit the @keyframes block from collected keyframes:
            builder.AppendAnimation(Name);

            // 2) Emit animation-* properties on the target:
            builder
                .Append("animation-name", Name)
                .AppendIfNotNull("animation-duration", CssTime(Duration))
                .AppendIfNotNull("animation-timing-function", TimingFunction)
                .AppendIfNotNull("animation-delay", CssTime(Delay))
                .AppendIf("animation-iteration-count", "infinite", Infinite)
                .AppendIfNotNull("animation-iteration-count", (!Infinite && IterationCount.HasValue) ? IterationCount.Value.ToString() : null)
                .AppendIfNotNull("animation-direction", Direction?.ToCss())
                .AppendIfNotNull("animation-fill-mode", FillMode?.ToCss())
                .AppendIfNotNull("animation-play-state", PlayState?.ToCss())
                .AppendIfNotNull("animation-composition", Composition);

            return builder;
        }

        private static string? CssTime(TimeSpan? t)
        {
            if (t is null) return null;
            var ts = t.Value;
            if (ts.TotalSeconds < 1) return $"{Math.Round(ts.TotalMilliseconds)}ms";
            var s = ts.TotalSeconds;
            return s % 1 == 0 ? $"{(int)s}s" : $"{s:0.###}s";
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<string>>(0);
            builder.AddAttribute(1, "Value", Name);
            builder.AddAttribute(2, "Name", nameof(Keyframe.AnimationName));
            builder.AddAttribute(3, "ChildContent", KeyFrames);
            builder.CloseComponent();
        }
    }

    public static class AnimationStyleExtensions
    {
        public static IStyleBuilder AnimationName(this IStyleBuilder b, string? name)
            => b.AppendIfNotNull("animation-name", name);

        public static IStyleBuilder AnimationDuration(this IStyleBuilder b, TimeSpan? t)
            => b.AppendIfNotNull("animation-duration", CssTime(t));

        public static IStyleBuilder AnimationDelay(this IStyleBuilder b, TimeSpan? t)
            => b.AppendIfNotNull("animation-delay", CssTime(t));

        public static IStyleBuilder AnimationTiming(this IStyleBuilder b, string? tf)
            => b.AppendIfNotNull("animation-timing-function", tf);

        public static IStyleBuilder AnimationIterationCount(this IStyleBuilder b, int? count, bool infinite = false)
        {
            if (infinite) return b.Append("animation-iteration-count", "infinite");
            return b.AppendIfNotNull("animation-iteration-count", count?.ToString());
        }

        public static IStyleBuilder AnimationDirection(this IStyleBuilder b, Animation.AnimationDirection? dir)
            => b.AppendIfNotNull("animation-direction", dir?.ToCss());

        public static IStyleBuilder AnimationFillMode(this IStyleBuilder b, Animation.AnimationFillMode? fill)
            => b.AppendIfNotNull("animation-fill-mode", fill?.ToCss());

        public static IStyleBuilder AnimationPlayState(this IStyleBuilder b, Animation.AnimationPlayState? st)
            => b.AppendIfNotNull("animation-play-state", st?.ToCss());

        /// <summary>
        /// Shorthand builder. Any null/unused part is omitted.
        /// </summary>
        public static IStyleBuilder Animation(this IStyleBuilder b,
            string name,
            TimeSpan? duration = null,
            string? timingFunction = null,
            TimeSpan? delay = null,
            int? iterationCount = null,
            bool infinite = false,
            Animation.AnimationDirection? direction = null,
            Animation.AnimationFillMode? fillMode = null,
            Animation.AnimationPlayState? playState = null,
            string? composition = null)
        {
            var parts = new List<string> { name };
            if (duration is not null) parts.Add(CssTime(duration ?? TimeSpan.MinValue));
            if (!string.IsNullOrWhiteSpace(timingFunction)) parts.Add(timingFunction!);
            if (delay is not null) parts.Add(CssTime(delay ?? TimeSpan.MinValue));
            if (infinite) parts.Add("infinite");
            else if (iterationCount is not null) parts.Add(iterationCount.Value.ToString());
            if (direction is not null) parts.Add(direction.Value.ToCss());
            if (fillMode is not null) parts.Add(fillMode.Value.ToCss());
            if (playState is not null) parts.Add(playState.Value.ToCss());
            if (!string.IsNullOrWhiteSpace(composition)) parts.Add(composition!);

            return b.Append("animation", string.Join(" ", parts));
        }

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
