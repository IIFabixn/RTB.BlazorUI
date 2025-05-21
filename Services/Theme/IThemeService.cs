using RTB.BlazorUI.Services.Theme.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme
{
    /// <summary>
    /// Theme provider interface.
    /// </summary>
    /// <typeparam name="TITHeme">Interface class of your Theme, which extends ITheme interface flag</typeparam>
    public interface IThemeService<TTheme> where TTheme : ITheme
    {
        /// <summary>
        /// Get the current theme.
        /// </summary>
        TTheme Current { get; }

        /// <summary>
        /// Event triggered when the theme changes.
        /// </summary>
        event Action? OnThemeChanged;

        /// <summary>
        /// Set the current theme.
        /// </summary>
        /// <param name="theme"></param>
        void SetTheme(TTheme theme);

        IList<TTheme> Themes { get; }
    }

    public interface IThemeServiceFactory
    {
        object GetCurrent();
    }
}