using RTB.Blazor.Styled.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Modules
{
    public sealed class AnimationModule : IStyleModule
    {
        private readonly Dictionary<string, List<(string offset, List<(string k, string v)>)>> _anims
            = new(StringComparer.OrdinalIgnoreCase);

        public bool HasInside => false;
        public bool HasOutside => _anims.Count > 0;

        public void Ensure(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            var key = name.Trim();
            if (!_anims.ContainsKey(key)) _anims[key] = [];
        }

        public void AppendFrame(string name, string offset, IEnumerable<(string k, string v)> props)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(offset)) return;
            var key = name.Trim();
            if (!_anims.TryGetValue(key, out var frames))
                _anims[key] = frames = [];

            frames.Add((offset.Trim(), props.ToList()));
        }

        public void Build(StringBuilder sb)
        {
            foreach (var (name, frames) in _anims)
            {
                sb.Append("@keyframes ").Append(name).Append('{');
                foreach (var (offset, props) in frames)
                {
                    sb.Append(offset).Append('{');
                    foreach (var (k, v) in props)
                        sb.Append(k).Append(':').Append(v).Append(';');
                    sb.Append('}');
                }
                sb.Append('}');
            }
        }

        public void Clear() => _anims.Clear();

        public void JoinFrom(IStyleModule other)
        {
            if (other is not AnimationModule o) return;
            foreach (var (name, frames) in o._anims)
            {
                if (!_anims.TryGetValue(name, out var mine))
                    _anims[name] = mine = [];
                mine.AddRange(frames);
            }
        }
    }
}
