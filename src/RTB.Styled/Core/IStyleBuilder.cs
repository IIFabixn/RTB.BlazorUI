using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    public interface IStyleBuilder
    {
        IStyleBuilder Append(string property, string value);
        IStyleBuilder AppendIf(string property, string? value, bool condition);
        IStyleBuilder AppendIfNotNull(string property, string? value);
        IStyleBuilder Join(params IStyleBuilder[] others);
        string Build();

        // Optional: decouple from UI framework
        void Register(IStyleContributor child);
        void Unregister(IStyleContributor child);
    }
}
