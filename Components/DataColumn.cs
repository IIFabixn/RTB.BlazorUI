using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Interfaces;

namespace RTB.BlazorUI.Components
{
    public interface IColumn<TRow> : IDisposable
    {
        IRegister<ColumnBase<TRow>>? ParentGrid { get; set; }
        void RenderHeader(RenderTreeBuilder builder, int col);
        void RenderCell(RenderTreeBuilder builder, TRow row, int col);
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

        public abstract void RenderHeader(RenderTreeBuilder builder, int col);
        public abstract void RenderCell(RenderTreeBuilder builder, TRow row, int col);
    }

    public class ViewColumn<TRow> : ColumnBase<TRow>, IDisposable
    {
        [Parameter, EditorRequired] public RenderFragment<TRow>? ChildContent { get; set; }

        public override void RenderHeader(RenderTreeBuilder builder, int col)
        {
            if (HeadContent is not null)
            {
                HeadContent.Invoke(builder);
            }

            // Add default content if HeadContent is null
            builder.OpenElement(0, "div");
            builder.AddContent(1, Name);
            builder.CloseElement();
        }

        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            if (ChildContent is not null)
                builder.AddContent(0, ChildContent(row));
        }
    }

    public class DataColumn<TRow, TValue> : ColumnBase<TRow>
    {
        [Inject] protected IStyled Styled { get; set; } = default!;

        [Parameter, EditorRequired] public Func<TRow, TValue> ValueFunc { get; set; } = default!;

        public override void RenderHeader(RenderTreeBuilder builder, int col)
        {
            // Add default content if HeadContent is null
            builder.OpenElement(0, "div");
            builder.AddContent(1, Name);
            builder.CloseElement();
        }

        protected override async Task OnParametersSetAsync()
        {
            ComponentClass = await Styled.CssAsync(StyleBuilder.Create()
            .Append("white-space", "nowrap")
            .Append("overflow", "hidden")
            .Append("text-overflow", "ellipsis")
            .Build());
        }

        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            var seq = 0;
            var value = ValueFunc(row);
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "role", "cell");
            builder.AddAttribute(seq++, "class", ClassBuilder.Create(ComponentClass).Append(CapturedAttributes.GetValueOrDefault<string>("class")).Build());
            builder.AddAttribute(seq++, "style", $"grid-column-start: {col}");
            builder.AddMultipleAttributes(seq++, CapturedAttributes);
            builder.AddContent(seq++, value);
            builder.CloseElement();
        }
    }
}
