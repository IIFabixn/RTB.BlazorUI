using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// A component that provides a scoped CSS class and style builder context to its children.
    /// </summary>
    public class Styled : ComponentBase
    {
        [Inject] private IStyleRegistry Registry { get; set; } = null!;

        /// <summary>
        /// Child content that receives the resolved CSS class as a parameter.
        /// </summary>
        [Parameter] public RenderFragment<string>? ChildContent { get; set; }

        /// <summary>
        /// Optional externally provided class name to use instead of generating a new one.
        /// </summary>
        [Parameter] public string? Classname { get; set; }

        /// <summary>
        /// Event callback that is invoked when the resolved class name changes.
        /// </summary>
        [Parameter] public EventCallback<string?> ClassnameChanged { get; set; }

        /// <summary>
        /// An optional action to configure the StyleBuilder used by this component.
        /// </summary>
        [Parameter] public Action<StyleBuilder>? Configure { get; set; }

        private readonly StyleBuilder _builder = StyleBuilder.Start;

        private string _resolvedClass = string.Empty;   // the class this component uses
        private string? _lastCss;         // memoized last emitted CSS

        /// <summary>
        /// After the component has rendered, configure the StyleBuilder, build the scoped CSS,
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Configure?.Invoke(_builder);
            _builder.Compose();
            // Build CSS already scoped to the resolved class
            var (cls, css) = _builder.BuildScoped(Classname);
            _resolvedClass = cls;
            if (string.IsNullOrEmpty(css))
                return;

            // Skip if nothing changed
            if (string.Equals(css, _lastCss, StringComparison.Ordinal))
                return;

            // Inject as-is (JS should *not* scope again; it should clear+append)
            await Registry.UpsertScopedAsync(css, _resolvedClass);

            _lastCss = css;

            // Keep two-way binding in sync (only if parent didn't provide a fixed Classname)
            if (Classname != _resolvedClass && ClassnameChanged.HasDelegate)
                await ClassnameChanged.InvokeAsync(_resolvedClass);

            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Render a CascadingValue that provides the StyleBuilder to descendants,
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<CascadingValue<StyleBuilder>>(0);
            builder.AddAttribute(1, "Value", _builder);
            builder.AddAttribute(2, "Name", nameof(StyleBuilder));
            builder.AddAttribute(3, "IsFixed", true);
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(b =>
            {
                if (ChildContent != null)
                    b.AddContent(0, ChildContent.Invoke(_resolvedClass));
            }));
            builder.CloseComponent();
        }

        /// <summary>
        /// Dispose the component by releasing the acquired class from the registry.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (!string.IsNullOrWhiteSpace(_resolvedClass))
                await Registry.Release(_resolvedClass);
        }
    }
}
