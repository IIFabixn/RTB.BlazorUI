using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    public interface IStyleFragment
    {
        void Emit(ScopedWriter w);
    }
}
