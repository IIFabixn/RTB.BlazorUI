using Microsoft.Extensions.DependencyInjection;
using RTB.Theme.Services.Theme;

namespace RTB.Theme.Extensions
{
    public static class RTBThemeExtension
    {
        public static IServiceCollection UseRTBTheme(this IServiceCollection collection, Type baseTheme)
        {
            var themeServiceType = typeof(RTBThemeService<>).MakeGenericType(baseTheme);
            collection.AddScoped(typeof(IThemeService<>).MakeGenericType(baseTheme), themeServiceType);

            return collection;
        }
    }
}
