using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    // A selector node, supports nesting via '&'
    public sealed class SelectorRule : IStyleFragment
    {
        public string Selector { get; }
        public DeclarationSet Declarations { get; init; } = new();
        public List<IStyleFragment> Children { get; init; } = [];

        public SelectorRule(string selector) => Selector = selector ?? string.Empty;

        public void Emit(ScopedWriter w)
        {
            foreach (var part in SplitSelectors(Selector))
            {
                var resolved = ResolveSelector(part, w.CurrentSelector);
                if (!Declarations.IsEmpty)
                    w.WriteRuleBlock(resolved, Declarations.ToDictionary(kv => kv.Key, kv => kv.Value));

                if (Children.Count > 0)
                    w.WithSelector(resolved, () => { foreach (var c in Children) c.Emit(w); });
            }
        }

        private static IEnumerable<string> SplitSelectors(string s) =>
            (s ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private static string ResolveSelector(string sel, string current) =>
            sel.StartsWith('&')
                ? sel.Replace("&", current)
                : string.IsNullOrWhiteSpace(sel) ? current : $"{current} {sel}";
    }
}
