using RTB.Blazor.Styled.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Modules
{
    public sealed class SelectorModule : IStyleModule
    {
        private readonly Dictionary<string, List<(string key, string value)>> _blocks
            = new(StringComparer.OrdinalIgnoreCase);

        public bool HasInside => _blocks.Count > 0;
        public bool HasOutside => false;

        public void Append(string selector, string property, string value)
        {
            if (string.IsNullOrWhiteSpace(selector) ||
                string.IsNullOrWhiteSpace(property) ||
                string.IsNullOrWhiteSpace(value)) return;

            var sel = selector.Trim();
            if (!_blocks.TryGetValue(sel, out var list))
                _blocks[sel] = list = [];
            list.Add((property.Trim(), value));
        }

        public void Build(StringBuilder sb)
        {
            foreach (var (selector, props) in _blocks)
            {
                sb.Append(selector).Append('{');
                foreach (var (k, v) in props)
                    sb.Append(k).Append(':').Append(v).Append(';');
                sb.Append('}');
            }
        }

        public void Clear() => _blocks.Clear();

        public void JoinFrom(IStyleModule other)
        {
            if (other is not SelectorModule o) return;
            foreach (var (sel, props) in o._blocks)
            {
                if (!_blocks.TryGetValue(sel, out var list))
                    _blocks[sel] = list = [];
                list.AddRange(props);
            }
        }
    }
}
