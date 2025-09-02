using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Services.Theme;

/// <summary>
/// Theme provider interface.
/// </summary>
/// <typeparam name="TTheme">
/// Interface type that represents a Theme and implements <see cref="ITheme"/>.
/// </typeparam>
/// <remarks>
/// Typical implementations:
/// - Expose a list of available themes via <see cref="Themes"/>.
/// - Track the currently selected theme via <see cref="Current"/>.
/// - Optionally persist the selection (e.g., to local storage in Blazor, preferences in MAUI).
/// - Raise <see cref="OnThemeChanged"/> after the theme is changed.
///
/// Threading:
/// - <see cref="OnThemeChanged"/> may be raised on a non-UI thread; consumers should marshal to the UI thread if required.
/// </remarks>
/// <example>
/// Example usage in a Blazor component:
/// <code>
/// @inject IThemeService&lt;MyTheme&gt; ThemeService
///
/// &lt;select @onchange="OnChange"&gt;
///     @foreach (var theme in ThemeService.Themes)
///     {
///         &lt;option selected="@(theme == ThemeService.Current)" value="@theme.Name"&gt;@theme.Name&lt;/option&gt;
///     }
/// &lt;/select&gt;
///
/// @code {
///     protected override void OnInitialized()
///     {
///         ThemeService.OnThemeChanged += StateHasChanged;
///     }
///
///     private async Task OnChange(ChangeEventArgs e)
///     {
///         var next = ThemeService.Themes.First(t =&gt; t.Name == (string)e.Value!);
///         await ThemeService.SetThemeAsync(next);
///     }
///
///     public void Dispose()
///     {
///         ThemeService.OnThemeChanged -= StateHasChanged;
///     }
/// }
/// </code>
/// </example>
public interface IThemeService<TTheme> where TTheme : ITheme
{
    /// <summary>
    /// Gets the currently active theme.
    /// </summary>
    TTheme Current { get; }

    /// <summary>
    /// Gets the default theme used when no persisted preference is available.
    /// </summary>
    TTheme Default { get; }

    /// <summary>
    /// Occurs after the current theme has changed.
    /// </summary>
    /// <remarks>
    /// Implementations should invoke this event after updating <see cref="Current"/>.
    /// Subscribers should assume it may be raised on a non-UI thread.
    /// </remarks>
    event Action? OnThemeChanged;

    /// <summary>
    /// Sets the current theme.
    /// </summary>
    /// <param name="theme">The theme to apply.</param>
    /// <returns>
    /// A task-like value representing the asynchronous operation. Implementations may persist
    /// the selection and apply side effects (e.g., updating CSS variables) before completing.
    /// </returns>
    ValueTask SetThemeAsync(TTheme theme);

    /// <summary>
    /// Gets the list of available themes.
    /// </summary>
    IList<TTheme> Themes { get; }
}

/// <summary>
/// Factory interface to access the current theme service instance without a generic type.
/// </summary>
/// <remarks>
/// Useful in scenarios where the concrete <c>TTheme</c> is not known at compile time,
/// such as dynamic composition or cross-cutting infrastructure.
/// </remarks>
public interface IThemeServiceFactory
{
    /// <summary>
    /// Gets the current theme service instance as an <see cref="object"/>.
    /// </summary>
    /// <returns>The current theme service instance; callers may cast to the expected generic interface.</returns>
    object GetCurrent();
}
