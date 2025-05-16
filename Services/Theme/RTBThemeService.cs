using RTB.BlazorUI.Services.Theme.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme
{
    public class RTBThemeService<TIThemeBase> : IThemeService<TIThemeBase> where TIThemeBase : ITheme
    {
        private TIThemeBase? _current;

        public TIThemeBase Current => _current ??= Themes.FirstOrDefault(t => t.GetType().GetCustomAttribute<ThemeAttribute>()?.IsDefault == true) ?? Themes.First();

        public IList<TIThemeBase> Themes => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(TIThemeBase)) && t.GetConstructor(Type.EmptyTypes) != null)
            .Select(t => (TIThemeBase)Activator.CreateInstance(t)!)
            .ToList();

        public event Action? OnThemeChanged;

        public void SetTheme(TIThemeBase theme)
        {
            _current = theme;
            OnThemeChanged?.Invoke();
        }
    }
}
