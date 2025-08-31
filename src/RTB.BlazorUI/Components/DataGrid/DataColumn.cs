using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styled.Services;
using RTB.Blazor.Interfaces;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Components.DataGrid
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
        Spacing[] Spacings { get; set; }
        Func<TRow, IComparable>? SortKey { get; set; }
        bool DefaultSortDescending { get; set; }
        bool SortDescending { get; set; }
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
        [Parameter] public Spacing[] Spacings { get; set; } = [];

        // Sorting
        [Parameter] public Func<TRow, IComparable>? SortKey { get; set; }
        [Parameter] public bool DefaultSortDescending { get; set; }
        public bool CanSort => SortKey is not null;
        public bool SortDescending { get; set; } = false;

        protected override void OnParametersSet()
        {
            ParentGrid?.Register(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                SortDescending = DefaultSortDescending;
            }
        }

        public void Dispose()
        {
            ParentGrid?.Unregister(this);
            GC.SuppressFinalize(this);
        }

        public abstract void RenderHeader(RenderTreeBuilder builder, int col);
        public abstract void RenderCell(RenderTreeBuilder builder, TRow row, int col);
    }

    public class ViewColumn<TRow> : ColumnBase<TRow>
    {
        [Parameter, EditorRequired] public RenderFragment<TRow> ChildContent { get; set; } = default!;

        public override void RenderHeader(RenderTreeBuilder builder, int col)
        {
            if (HeadContent is not null)
            {
                HeadContent.Invoke(builder);
                return;
            }

            // Add default content if HeadContent is null
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", $"padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");
            builder.AddContent(2, Name);
            builder.CloseElement();
        }

        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            var seq = 0;
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", CombineClass("rtb-viewcolumn", Class));
            builder.AddAttribute(seq++, "style", $"grid-column-start: {col}; min-height: fit-content;padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");
            builder.AddContent(seq++,ChildContent(row));
            builder.CloseElement();
        }
    }

    public class DataColumn<TRow, TValue> : ColumnBase<TRow>
    {
        [Inject] private IStyleRegistry Registry { get; set; } = default!;

        [Parameter, EditorRequired] public Func<TRow, TValue> ValueFunc { get; set; } = default!;

        private string? CellClass;

        private readonly string style = StyleBuilder.Start
            .Append("white-space", "nowrap")
            .Append("overflow", "hidden")
            .Append("text-overflow", "ellipsis")
            .Build();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            
            CellClass ??= Registry.GetOrCreate(style);
            await Registry.InjectInto(style, CellClass);
        }

        public override void RenderHeader(RenderTreeBuilder builder, int col)
        {
            // Add default content if HeadContent is null
            var seq = 0;
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "style", $"padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");
            builder.AddContent(seq++, Name);
            builder.CloseElement();
        }

        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            var seq = 0;
            var value = ValueFunc(row);
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "role", "cell");
            builder.AddAttribute(seq++, "class", CombineClass("rtb-datacolumn", CellClass, Class));
            builder.AddAttribute(seq++, "style", $"grid-column-start: {col}; min-height: fit-content;padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");

            builder.AddContent(seq++, value);
            builder.CloseElement();
        }
    }
}
