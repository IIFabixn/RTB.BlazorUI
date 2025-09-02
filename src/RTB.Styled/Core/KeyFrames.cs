using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// Represents a CSS <c>@keyframes</c> rule and its frames.
    /// </summary>
    /// <remarks>
    /// - Emits compact CSS without validation or escaping.
    /// - Nothing is emitted if <see cref="Name"/> is null/whitespace or if there are no frames with declarations.
    /// - The order of frames in <see cref="Frames"/> is preserved on emission.
    /// </remarks>
    public sealed class Keyframes : IStyleFragment
    {
        /// <summary>
        /// The animation name (CSS identifier) for the <c>@keyframes</c> rule.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The ordered collection of frames contained in this <c>@keyframes</c> rule.
        /// </summary>
        /// <remarks>
        /// No validation is performed on offsets or duplicate entries.
        /// </remarks>
        public List<KeyframeFrame> Frames { get; } = [];

        /// <summary>
        /// Creates a new <see cref="Keyframes"/> with the provided animation name.
        /// </summary>
        /// <param name="name">The animation name (CSS identifier). Not validated.</param>
        public Keyframes(string name) => Name = name;

        /// <summary>
        /// Emits this <c>@keyframes</c> rule and its frames to the provided writer.
        /// </summary>
        /// <param name="w">The scoped writer to receive CSS output.</param>
        /// <remarks>
        /// Skips emission if <see cref="Name"/> is null/whitespace or <see cref="Frames"/> is empty.
        /// </remarks>
        public void Emit(ScopedWriter w)
        {
            if (string.IsNullOrWhiteSpace(Name) || Frames.Count == 0) return;
            w.Write($"@keyframes {Name}{{");
            foreach (var f in Frames) f.Emit(w);
            w.Write("}");
        }

        /// <summary>
        /// Adds a frame to this <c>@keyframes</c> rule.
        /// </summary>
        /// <param name="frame">The frame to add. Ignored if null.</param>
        /// <returns>The current <see cref="Keyframes"/> instance for chaining.</returns>
        public Keyframes Add(KeyframeFrame frame)
        {
            if (frame != null) Frames.Add(frame);
            return this;
        }
    }

    /// <summary>
    /// Represents a single frame within a <c>@keyframes</c> rule.
    /// </summary>
    /// <remarks>
    /// The <see cref="Offset"/> should be a valid CSS keyframe selector such as <c>"from"</c>, <c>"to"</c>, or a percentage like <c>"0%"</c>, <c>"50%"</c>, <c>"100%"</c>.
    /// If <see cref="Declarations"/> is empty, this frame is not emitted.
    /// </remarks>
    public sealed class KeyframeFrame : IStyleFragment
    {
        /// <summary>
        /// The keyframe offset selector (e.g., <c>"0%"</c>, <c>"50%"</c>, <c>"100%"</c>, <c>"from"</c>, or <c>"to"</c>).
        /// </summary>
        public string Offset { get; }

        /// <summary>
        /// The set of CSS declarations for this frame.
        /// </summary>
        /// <remarks>
        /// Declarations are emitted in the enumeration order of the underlying set.
        /// </remarks>
        public DeclarationSet Declarations { get; } = [];

        /// <summary>
        /// Creates a new <see cref="KeyframeFrame"/> for the specified offset.
        /// </summary>
        /// <param name="offset">The keyframe offset (not validated).</param>
        public KeyframeFrame(string offset) => Offset = offset;

        /// <summary>
        /// Emits this frame's declarations to the provided writer.
        /// </summary>
        /// <param name="w">The scoped writer to receive CSS output.</param>
        /// <remarks>
        /// Takes a snapshot of <see cref="Declarations"/> to avoid issues if the set is modified during iteration.
        /// </remarks>
        public void Emit(ScopedWriter w)
        {
            if (Declarations.IsEmpty) return;
            w.Write($"{Offset}{{");
            // Write from an in-memory snapshot to be safe during iteration
            w.WriteDeclarations(Declarations.ToDictionary(kv => kv.Key, kv => kv.Value));
            w.Write("}");
        }
    }
}
