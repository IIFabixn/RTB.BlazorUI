using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    public interface IStyleModule
    {
        bool HasInside { get; }
        bool HasOutside { get; }
        void BuildInside(StringBuilder sb);
        void BuildOutside(StringBuilder sb);
        void Clear();
        void JoinFrom(IStyleModule other);
    }
}
