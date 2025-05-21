using RTB.BlazorUI.Services.Theme.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme
{
    public class RTBThemeService<TThemeBase> : IThemeService<TThemeBase> where TThemeBase : ITheme
    {
        private TThemeBase? _current;

        public TThemeBase Current => _current ??= Themes.FirstOrDefault(t => t.GetType().GetCustomAttribute<ThemeAttribute>()?.IsDefault == true) ?? Themes.First();

        public IList<TThemeBase> Themes => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(TThemeBase)) && t.GetConstructor(Type.EmptyTypes) != null)
            .Select(t => (TThemeBase)Activator.CreateInstance(t)!)
            .ToList();

        public event Action? OnThemeChanged;

        public void SetTheme(TThemeBase theme)
        {
            _current = theme;
            OnThemeChanged?.Invoke();
        }
    }
}