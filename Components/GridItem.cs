using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Components
{
    /// <summary>
    /// Helper Component to register itself in the parent Grid.
    /// </summary>
    public class GridItem : RTBComponent, IDisposable
    {
        [CascadingParameter] public Grid Parent { get; set; } = null!;
        [Parameter] public int Column { get; set; } = 1;
        [Parameter] public int ColumnSpan { get; set; } = 1;
        [Parameter] public int Row { get; set; } = 1;
        [Parameter] public int RowSpan { get; set; } = 1;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        protected override void OnParametersSet()
        {
            Parent.RegisterItem(this);
        }

        public void Dispose()
        {
            Parent.UnregisterItem(this);
        }
    }
}
