using Microsoft.JSInterop;
using RTB.Blazor.Services.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Services.Theme;

/// <summary>
/// Provides theme discovery, selection, and persistence for Blazor using JS interop.
/// </summary>
/// <typeparam name="TThemeBase">
/// Base theme interface that must implement <see cref="ITheme"/>. All concrete types assignable to this type
/// with a public parameterless constructor are discovered via reflection as available themes.
/// </typeparam>
/// <param name="jsRuntime">
/// JS runtime used to persist the selected theme to browser localStorage (key: "rtbtheme").
/// </param>
public class RTBThemeService<TThemeBase>(IJSRuntime jsRuntime) : IThemeService<TThemeBase> where TThemeBase : ITheme
{
    private TThemeBase? _current;

    /// <summary>
    /// Gets the default theme.
    /// Determined by the presence of <see cref="ThemeAttribute"/> with <c>IsDefault == true</c>,
    /// otherwise falls back to the first discovered theme.
    /// </summary>
    public TThemeBase Default => Themes.FirstOrDefault(t => t.GetType().GetCustomAttribute<ThemeAttribute>()?.IsDefault == true) ?? Themes.First();

    /// <summary>
    /// Gets the current theme. If not explicitly set, this returns <see cref="Default"/>.
    /// </summary>
    public TThemeBase Current => _current ??= Default ?? Themes.First();

    /// <summary>
    /// Gets all available theme instances discovered via reflection.
    /// A theme is considered available if it:
    /// - Is a non-abstract class
    /// - Is assignable to <typeparamref name="TThemeBase"/>
    /// - Has a public parameterless constructor
    /// Each matching type is instantiated once when this property is evaluated.
    /// </summary>
    public IList<TThemeBase> Themes => [.. AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(TThemeBase)) && t.GetConstructor(Type.EmptyTypes) != null)
        .Select(t => (TThemeBase)Activator.CreateInstance(t)! )];

    /// <summary>
    /// Raised after the current theme has changed via <see cref="SetThemeAsync(TThemeBase)"/>.
    /// </summary>
    public event Action? OnThemeChanged;

    /// <summary>
    /// Sets the current theme and persists the selection to browser localStorage using JS interop.
    /// </summary>
    /// <param name="theme">The theme instance to set as current.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public ValueTask SetThemeAsync(TThemeBase theme)
    {
        _current = theme;
        OnThemeChanged?.Invoke();
        return jsRuntime.InvokeVoidAsync("localStorage.setItem", "rtbtheme", theme.GetType().Name);
    }
}