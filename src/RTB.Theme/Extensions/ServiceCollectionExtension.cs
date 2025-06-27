using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RTB.Blazor.Theme.Services;

namespace RTB.Blazor.Theme.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection UseRTBTheme(this IServiceCollection collection, Type baseTheme)
        {
            var themeServiceType = typeof(RTBThemeService<>).MakeGenericType(baseTheme);
            collection.TryAddScoped(typeof(IThemeService<>).MakeGenericType(baseTheme), themeServiceType);

            return collection;
        }
    }
}
