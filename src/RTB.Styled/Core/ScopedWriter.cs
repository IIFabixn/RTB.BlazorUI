using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    public sealed class ScopedWriter
    {
        private readonly StringBuilder _sb;
        private readonly Stack<string> _selectorStack = new();

        public string CurrentSelector => _selectorStack.Peek();

        public ScopedWriter(StringBuilder sb, string rootSelector)
        {
            _sb = sb;
            _selectorStack.Push(rootSelector);
        }

        public void WithSelector(string selector, Action emit)
        {
            _selectorStack.Push(selector);
            try { emit(); } finally { _selectorStack.Pop(); }
        }

        public void Write(string s) => _sb.Append(s);

        public void WriteRuleBlock(string selector, IReadOnlyDictionary<string, string> decls)
        {
            _sb.Append(selector).Append('{');
            WriteDeclarations(decls);
            _sb.Append('}');
        }

        public void WriteDeclarations(IReadOnlyDictionary<string, string> decls)
        {
            foreach (var kv in decls)
            {
                _sb.Append(kv.Key).Append(':').Append(kv.Value).Append(';');
            }
        }
    }
}
