using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RTB.Blazor.Styled.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection UseRTBStyled(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IStyleRegistry, StyleRegistry>();

            return collection;
        }
    }
}
