using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Interfaces;

namespace RTB.BlazorUI.Components
{
    /// <summary>
    /// Helper Component to register itself in the parent Grid.
    /// </summary>
    public class GridItem : RTBComponent, IDisposable
    {
        public readonly Guid Guid = Guid.NewGuid();

        [CascadingParameter] public IRegister<GridItem> Parent { get; set; } = null!;
        [Parameter] public int Column { get; set; } = -1;
        [Parameter] public int ColumnSpan { get; set; } = 1;
        [Parameter] public int Row { get; set; } = -1;
        [Parameter] public int RowSpan { get; set; } = 1;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        protected override void OnParametersSet()
        {
            Parent.Register(this);
        }

        public void Dispose()
        {
            Parent.Unregister(this);
            GC.SuppressFinalize(this);
        }
    }
}
