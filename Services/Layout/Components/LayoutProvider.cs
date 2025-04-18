using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Sections;

namespace RTB.BlazorUI.Services.Layout.Components
{
    /// <summary>
    /// This component is used to provide content to a layout component.<br/>
    /// Obsolete due to <see cref="SectionOutlet"/> and <see cref="SectionContent"/> in Blazor.
    /// </summary>
    /// <typeparam name="TLayout"></typeparam>
    /// <param name="layoutService"></param>
    [Obsolete("Obsolete due to SectionOutlet and SectionContent in Blazor.")]
    public class LayoutProvider<TLayout>(LayoutService layoutService) : ComponentBase where TLayout : LayoutComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter] public string LayoutPropertyName { get; set; } = string.Empty;

        protected override void OnParametersSet()
        {
            layoutService.ProvideContent<TLayout>(ChildContent, LayoutPropertyName);
        }
    }
}
