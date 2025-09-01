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
        public static StyleBuilder AnimationName(this StyleBuilder b, string? name)
            => b.SetIfNotNull("animation-name", name);

        public static StyleBuilder AnimationDuration(this StyleBuilder b, TimeSpan? t)
            => b.Set("animation-duration", CssTime(t));

        public static StyleBuilder AnimationDelay(this StyleBuilder b, TimeSpan? t)
            => b.Set("animation-delay", CssTime(t));

        public static StyleBuilder AnimationTiming(this StyleBuilder b, string? tf)
            => b.SetIfNotNull("animation-timing-function", tf);

        public static StyleBuilder AnimationIterationCount(this StyleBuilder b, int? count, bool infinite = false)
        {
            if (infinite) return b.Set("animation-iteration-count", "infinite");
            return b.SetIfNotNull("animation-iteration-count", count?.ToString());
        }

        public static StyleBuilder AnimationDirection(this StyleBuilder b, Animation.AnimationDirection? dir)
            => b.SetIfNotNull("animation-direction", dir?.ToCss());

        public static StyleBuilder AnimationFillMode(this StyleBuilder b, Animation.AnimationFillMode? fill)
            => b.SetIfNotNull("animation-fill-mode", fill?.ToCss());

        public static StyleBuilder AnimationPlayState(this StyleBuilder b, Animation.AnimationPlayState? st)
            => b.SetIfNotNull("animation-play-state", st?.ToCss());
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
