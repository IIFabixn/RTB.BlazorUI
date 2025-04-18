using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Components;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components.Sections;

namespace RTB.BlazorUI.Services.Layout
{
    /// <summary>
    /// This service allows you to register a layout component and provide content to its sections.<br/>
    /// Obsolete due to <see cref="SectionOutlet"/> and <see cref="SectionContent"/> in Blazor.
    /// </summary>
    [Obsolete("Obsolete due to SectionOutlet and SectionContent in Blazor.")]
    public class LayoutService
    {
        private readonly Dictionary<Type, Action<RenderFragment, string>?> _layoutCallbacks = [];
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _layoutSectionProperties = [];

        public void Register<TLayout>(TLayout layout, Action<RenderFragment, string>? callback = null) where TLayout : LayoutComponentBase
        {
            var layoutType = typeof(TLayout);
            
            // Gather all [LayoutSection] properties
            var sectionProps = layoutType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.PropertyType == typeof(RenderFragment) && p.GetCustomAttribute<LayoutSectionAttribute>() != null)
                .ToDictionary(
                    p =>
                    {
                        var attr = p.GetCustomAttribute<LayoutSectionAttribute>()!;
                        return attr.SectionName ?? p.Name;
                    },
                    p => p
                );

            // Cache the layout section properties for safety
            _layoutSectionProperties[layoutType] = sectionProps;
            _layoutCallbacks[layoutType] = callback ?? DefaultCallback(layout);
        }

        public void Unregister<TLayout>(TLayout layout) where TLayout : LayoutComponentBase
        {
            _layoutCallbacks.Remove(typeof(TLayout));
        }

        public void ProvideContent<TLayout>(RenderFragment content, string sectionName) where TLayout : LayoutComponentBase
        {
            var layoutType = typeof(TLayout);
            if (_layoutCallbacks.TryGetValue(layoutType, out var callback))
            {
                callback?.Invoke(content, sectionName);
            }
        }

        private Action<RenderFragment, string> DefaultCallback<TLayout>(TLayout layout) where TLayout : LayoutComponentBase
        {
            var layoutType = typeof(TLayout);
            return (content, sectionName) =>
            {
                if (!_layoutSectionProperties[layoutType].TryGetValue(sectionName, out var prop))
                    throw new InvalidOperationException($"'{sectionName}' is not a valid layout section for {layoutType.Name}.");

                prop.SetValue(layout, content);

                layout.GetType()
                    .GetMethod("StateHasChanged", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?.Invoke(layout, null);
            };
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LayoutSectionAttribute : Attribute
    {
        public string? SectionName { get; }

        public LayoutSectionAttribute() { }

        public LayoutSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }
}
