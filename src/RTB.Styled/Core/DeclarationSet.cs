using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    // A bag of declarations destined for the *current* selector
    public sealed class DeclarationSet : IStyleFragment, IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _decls = new(StringComparer.Ordinal);
        public bool IsEmpty => _decls.Count == 0;

        public void Add(string prop, string value)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value)) return;
            _decls[prop.Trim()] = value.Trim(); // last-write-wins
        }

        public void Join(IEnumerable<KeyValuePair<string, string>> source)
        {
            if (source == null) return;
            foreach (var (k, v) in source) Add(k, v);
        }

        public void Emit(ScopedWriter w)
        {
            if (IsEmpty) return;
            w.WriteRuleBlock(w.CurrentSelector, _decls);
        }

        public void Clear() => _decls.Clear();

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _decls.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
