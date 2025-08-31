using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    internal interface IStyleSnapshot
    {
        IReadOnlyDictionary<string, string> Props { get; }
        IReadOnlyDictionary<Type, IStyleModule> Modules { get; }
        IEnumerable<IStyleContributor> Children { get; }
    }
}
