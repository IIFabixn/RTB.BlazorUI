using RTB.Blazor.Styled.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Modules
{
    public sealed class MediaModule : IStyleModule
    {
        // media query → list of raw CSS blocks (already wrapped by caller or structured via selectors)
        private readonly Dictionary<string, List<string>> _medias = new(StringComparer.OrdinalIgnoreCase);

        public bool HasInside => false;
        public bool HasOutside => _medias.Count > 0;

        public void Append(string mediaQuery, string cssBlock)
        {
            if (string.IsNullOrWhiteSpace(mediaQuery) || string.IsNullOrWhiteSpace(cssBlock)) return;
            if (!_medias.TryGetValue(mediaQuery, out var list))
                _medias[mediaQuery] = list = [];
            list.Add(cssBlock);
        }

        public void BuildInside(StringBuilder sb) => throw new NotImplementedException();
        public void BuildOutside(StringBuilder sb)
        {
            foreach (var (mq, blocks) in _medias)
            {
                sb.Append(mq).Append('{');
                foreach (var b in blocks) sb.Append(b);
                sb.Append('}');
            }
        }

        public void Clear() => _medias.Clear();

        public void JoinFrom(IStyleModule other)
        {
            if (other is not MediaModule o) return;
            foreach (var (mq, blocks) in o._medias)
            {
                if (!_medias.TryGetValue(mq, out var list))
                    _medias[mq] = list = [];
                list.AddRange(blocks);
            }
        }
    }
}
