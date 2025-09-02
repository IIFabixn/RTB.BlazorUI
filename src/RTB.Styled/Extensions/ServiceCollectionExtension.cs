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
    /// <summary>
    /// Provides extension methods for registering RTB.Styled services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registers RTB.Styled services in the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection">
        /// The service collection to which RTB.Styled services will be added.
        /// </param>
        /// <returns>
        /// The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.
        /// </returns>
        /// <remarks>
        /// This method registers:
        /// - <see cref="IStyleRegistry"/> with a scoped lifetime using <see cref="StyleRegistry"/>.
        /// 
        /// It is safe to call multiple times; services are only added if they have not already been registered.
        /// </remarks>
        /// <example>
        /// In Program.cs (Blazor or ASP.NET Core):
        /// <code>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.Services.UseRTBStyled();
        /// </code>
        /// </example>
        public static IServiceCollection UseRTBStyled(this IServiceCollection collection)
        {
            collection.TryAddScoped<IStyleRegistry, StyleRegistry>();

            return collection;
        }
    }
}
