using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    // @keyframes
    public sealed class Keyframes : IStyleFragment
    {
        public string Name { get; }
        public List<KeyframeFrame> Frames { get; } = [];

        public Keyframes(string name) => Name = name;

        public void Emit(ScopedWriter w)
        {
            if (string.IsNullOrWhiteSpace(Name) || Frames.Count == 0) return;
            w.Write($"@keyframes {Name}{{");
            foreach (var f in Frames) f.Emit(w);
            w.Write("}");
        }

        public Keyframes Add(KeyframeFrame frame)
        {
            if (frame != null) Frames.Add(frame);
            return this;
        }
    }

    public sealed class KeyframeFrame : IStyleFragment
    {
        public string Offset { get; } // "0%", "to", "from", "50%"
        public DeclarationSet Declarations { get; } = [];

        public KeyframeFrame(string offset) => Offset = offset;

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
