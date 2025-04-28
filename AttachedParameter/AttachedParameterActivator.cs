using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.AttachedParameter
{
    public class AttachedParameterACtivator(IServiceProvider serviceProvider) : IComponentActivator
    {
        private static readonly ConcurrentDictionary<Type, ObjectFactory> _cachedComponentTypeInfo = new();

        public static void ClearCache() => _cachedComponentTypeInfo.Clear();

        private readonly Dictionary<Type, IEnumerable<PropertyInfo>> attachedParametersCache = [];
        public IComponent CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type componentType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException($"The type {componentType.FullName} does not implement {nameof(IComponent)}.", nameof(componentType));
            }

            if (!attachedParametersCache.TryGetValue(componentType, out var namedParameters))
            {
                namedParameters = componentType.GetProperties().Where(p => p.GetCustomAttributes<AttachedParameterAttribute>() != null);
                attachedParametersCache[componentType] = namedParameters;
            }

            var factory = GetObjectFactory(componentType);

            return (IComponent)factory(serviceProvider, []);
        }
        private static ObjectFactory GetObjectFactory([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type componentType)
        {
            // Unfortunately we can't use 'GetOrAdd' here because the DynamicallyAccessedMembers annotation doesn't flow through to the
            // callback, so it becomes an IL2111 warning. The following is equivalent and thread-safe because it's a ConcurrentDictionary
            // and it doesn't matter if we build a cache entry more than once.
            if (!_cachedComponentTypeInfo.TryGetValue(componentType, out var factory))
            {
                factory = ActivatorUtilities.CreateFactory(componentType, Type.EmptyTypes);
                _cachedComponentTypeInfo.TryAdd(componentType, factory);
            }

            return factory;
        }
    }
}
