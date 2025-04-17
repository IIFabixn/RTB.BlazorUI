using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RTB.BlazorUI.Components
{
    public interface IDataColumn<TRow>
    {
        void RenderHeader(RenderTreeBuilder builder);
        void RenderCell(RenderTreeBuilder builder, TRow row);
        string? Width { get; set; }
        bool DefaultSortDescending { get; set; }
        Func<TRow, IComparable>? SortKey { get; set; }
        bool CanSort { get; }
    }
    
    public class DataColumn<TRow, TValue> : RTBComponent, IDataColumn<TRow>
    {
        [CascadingParameter] public DataGrid<TRow>? ParentGrid { get; set; }
        [Parameter] public string Name { get; set; } = string.Empty;
        [Parameter, EditorRequired] public Func<TRow, TValue> ValueFunc { get; set; } = default!;
        [Parameter] public RenderFragment<TValue>? ChildContent { get; set; }
        [Parameter] public RenderFragment? HeadContent { get; set; }
        [Parameter] public string? Width { get; set; }
        [Parameter] public Func<TRow, IComparable>? SortKey { get; set; }
        [Parameter] public bool DefaultSortDescending { get; set; }
        public bool CanSort => SortKey is not null;

        protected override void OnParametersSet()
        {
            ParentGrid?.AddColumn(this);
        }

        public void Dispose() => ParentGrid?.RemoveColumn(this);

        public void RenderHeader(RenderTreeBuilder builder)
        {
            if (HeadContent is not null)
            {
                HeadContent.Invoke(builder);
                return;
            }

            // Add default content if HeadContent is null
            builder.OpenElement(0, "div");
            builder.AddContent(1, Name);
            builder.CloseElement();
        }

        public void RenderCell(RenderTreeBuilder builder, TRow row)
        {
            var value = ValueFunc(row);
            if (ChildContent is not null)
                builder.AddContent(0, ChildContent(value));
            else
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "whitespace-nowrap");
                builder.AddContent(2, value);
                builder.CloseElement();
            }
        }
    }
}
