using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    // A grouping rule like @media / @supports / @container
    public sealed class GroupRule : IStyleFragment
    {
        public string Kind { get; }      // "@media", "@supports", "@container", "@layer"
        public string Prelude { get; }   // e.g., "screen and (min-width: 992px)"
        public List<IStyleFragment> Children { get; } = new();

        public GroupRule(string kind, string prelude)
        {
            Kind = kind; Prelude = prelude ?? string.Empty;
        }

        public void Emit(ScopedWriter w)
        {
            if (Children.Count == 0) return;
            w.Write($"{Kind}{(string.IsNullOrWhiteSpace(Prelude) ? "" : " " + Prelude)}{{");
            foreach (var c in Children) c.Emit(w); // same CurrentSelector
            w.Write("}");
        }
    }
}
