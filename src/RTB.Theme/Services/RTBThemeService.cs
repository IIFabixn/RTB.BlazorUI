using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Theme.Services;

public class RTBThemeService<TThemeBase>(IJSRuntime jsRuntime) : IThemeService<TThemeBase> where TThemeBase : ITheme
{
    private TThemeBase? _current;

    public TThemeBase Default => Themes.FirstOrDefault(t => t.GetType().GetCustomAttribute<ThemeAttribute>()?.IsDefault == true) ?? Themes.First();

    public TThemeBase Current => _current ??= Default ?? Themes.First();

    public IList<TThemeBase> Themes => [.. AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(TThemeBase)) && t.GetConstructor(Type.EmptyTypes) != null)
        .Select(t => (TThemeBase)Activator.CreateInstance(t)!)];

    public event Action? OnThemeChanged;

    public ValueTask SetThemeAsync(TThemeBase theme)
    {
        _current = theme;
        OnThemeChanged?.Invoke();
        return jsRuntime.InvokeVoidAsync("localStorage.setItem", "rtbtheme", theme.GetType().Name);
    }
}