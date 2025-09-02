using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Interfaces
{
    /// <summary>
    /// Defines a contract for components that can register and unregister items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item to register and unregister. Implementations may constrain or validate this type.
    /// </typeparam>
    /// <remarks>
    /// Implementations should document thread-safety and whether operations are idempotent.
    /// </remarks>
    public interface IRegister<T>
    {
        /// <summary>
        /// Registers the specified item with the implementing component.
        /// </summary>
        /// <param name="item">
        /// The item to register. When <typeparamref name="T"/> is a reference type, this parameter should not be null.
        /// </param>
        void Register(T item);

        /// <summary>
        /// Unregisters the specified item from the implementing component.
        /// </summary>
        /// <param name="item">
        /// The item to unregister. When <typeparamref name="T"/> is a reference type, this parameter should not be null.
        /// </param>
        void Unregister(T item);
    }
}
