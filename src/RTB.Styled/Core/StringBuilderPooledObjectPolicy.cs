using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    internal sealed class StringBuilderPooledObjectPolicy : PooledObjectPolicy<StringBuilder>
    {
        public int InitialCapacity { get; set; } = 256;
        public int MaximumRetainedCapacity { get; set; } = 1024;

        public override StringBuilder Create() => new(InitialCapacity);

        public override bool Return(StringBuilder obj)
        {
            if (obj.Capacity > MaximumRetainedCapacity) return false;
            obj.Clear();
            return true;
        }
    }
}
