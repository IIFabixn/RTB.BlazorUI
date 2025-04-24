using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Interfaces;

namespace RTB.BlazorUI.Components
{
    public interface IColumn<TRow> : IDisposable
    {
        IRegister<ColumnBase<TRow>>? ParentGrid { get; set; }
        void RenderHeader(RenderTreeBuilder builder);
        void RenderCell(RenderTreeBuilder builder, TRow row);
        string Name { get; set; }
        string? Width { get; set; }
        string? MinWidth { get; set; }
        string? MaxWidth { get; set; }
        Func<TRow, IComparable>? SortKey { get; set; }
        bool DefaultSortDescending { get; set; }
        bool CanSort => SortKey is not null;
    }

    public abstract class ColumnBase<TRow> : RTBComponent, IColumn<TRow>
    {
        public readonly Guid Guid = Guid.NewGuid();

        [CascadingParameter] public IRegister<ColumnBase<TRow>>? ParentGrid { get; set; }
        [Parameter] public string Name { get; set; } = string.Empty;
        [Parameter] public RenderFragment? HeadContent { get; set; }
        [Parameter] public string? Width { get; set; }
        [Parameter] public string? MinWidth { get; set; }
        [Parameter] public string? MaxWidth { get; set; }
        [Parameter] public Func<TRow, IComparable>? SortKey { get; set; }
        [Parameter] public bool DefaultSortDescending { get; set; }
        public bool CanSort => SortKey is not null;
        
        protected override void OnParametersSet()
        {
            ParentGrid?.Register(this);
        }

        public void Dispose() => ParentGrid?.Unregister(this);

        public abstract void RenderHeader(RenderTreeBuilder builder);
        public abstract void RenderCell(RenderTreeBuilder builder, TRow row);
    }

    public class ViewColumn<TRow> : ColumnBase<TRow>, IDisposable
    {
        [Parameter, EditorRequired] public RenderFragment<TRow>? ChildContent { get; set; }

        public override void RenderHeader(RenderTreeBuilder builder)
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

        public override void RenderCell(RenderTreeBuilder builder, TRow row)
        {
            if (ChildContent is not null)
                builder.AddContent(0, ChildContent(row));
        }
    }

    public class DataColumn<TRow, TValue> : ColumnBase<TRow>
    {
        [Parameter, EditorRequired] public Func<TRow, TValue> ValueFunc { get; set; } = default!;
        [Parameter] public RenderFragment<TValue>? ChildContent { get; set; }

        public override void RenderHeader(RenderTreeBuilder builder)
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

        public override void RenderCell(RenderTreeBuilder builder, TRow row)
        {
            var value = ValueFunc(row);
            if (ChildContent is not null)
                builder.AddContent(0, ChildContent(value));
            else
            {
                builder.OpenElement(0, "div");
                builder.AddContent(2, value);
                builder.CloseElement();
            }
        }
    }
}
