using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Interfaces
{
    public interface IRegister<T>
    {
        void Register(T item);
        void Unregister(T item);
    }
}
