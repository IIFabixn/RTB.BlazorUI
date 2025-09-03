using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styled.Services;
using RTB.Blazor.Interfaces;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Components.DataGrid
{
    /// <summary>
    /// Column interface for DataGrid.
    /// </summary>
    /// <typeparam name="TRow"></typeparam>
    public interface IColumn<TRow> : IDisposable
    {
        /// <summary>
        /// Reference to the parent grid for registration.
        /// </summary>
        IRegister<ColumnBase<TRow>>? ParentGrid { get; set; }

        /// <summary>
        /// Renders the header for the column.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="col"></param>
        void RenderHeader(RenderTreeBuilder builder, int col);

        /// <summary>
        /// Renders a cell for the given row and column index.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        void RenderCell(RenderTreeBuilder builder, TRow row, int col);

        /// <summary>
        /// The unique identifier for the column.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Optional content for the column header.
        /// </summary>
        string? Width { get; set; }

        /// <summary>
        /// Optional minimum width for the column.
        /// </summary>
        string? MinWidth { get; set; }

        /// <summary>
        /// Optional maximum width for the column.
        /// </summary>
        string? MaxWidth { get; set; }

        /// <summary>
        /// Optional spacings for the column header.
        /// </summary>
        Spacing[] Spacings { get; set; }

        /// <summary>
        /// Optional Sorting key function for the column.
        /// </summary>
        Func<TRow, IComparable>? SortKey { get; set; }

        /// <summary>
        /// Indicates if the default sort order is descending.
        /// </summary>
        bool DefaultSortDescending { get; set; }

        /// <summary>
        /// Indicates if the current sort order is descending.
        /// </summary>
        bool SortDescending { get; set; }

        /// <summary>
        /// Indicates if the column can be sorted.
        /// </summary>
        bool CanSort => SortKey is not null;
    }

    /// <summary>
    /// Base class for DataGrid columns.
    /// </summary>
    /// <typeparam name="TRow"></typeparam>
    public abstract class ColumnBase<TRow> : RTBComponent, IColumn<TRow>
    {
        /// <summary>
        /// The unique identifier for the column.
        /// </summary>
        public readonly Guid Guid = Guid.NewGuid();

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.ParentGrid"/>
        /// </summary>
        [CascadingParameter] public IRegister<ColumnBase<TRow>>? ParentGrid { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.Name"/>
        /// </summary>
        [Parameter] public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Custom content for the column header.
        /// </summary>
        [Parameter] public RenderFragment? HeadContent { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.Width"/>
        /// </summary>
        [Parameter] public string? Width { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.MinWidth"/>
        /// </summary>
        [Parameter] public string? MinWidth { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.MaxWidth"/>
        /// </summary>
        [Parameter] public string? MaxWidth { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.Spacings"/>
        /// </summary>
        [Parameter] public Spacing[] Spacings { get; set; } = [];

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.SortKey"/>
        /// </summary>
        [Parameter] public Func<TRow, IComparable>? SortKey { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.DefaultSortDescending"/>
        /// </summary>
        [Parameter] public bool DefaultSortDescending { get; set; }

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.SortKey"/>
        /// </summary>
        public bool CanSort => SortKey is not null;

        /// <summary>
        /// <inheritdoc cref="IColumn{TRow}.SortDescending"/>
        /// </summary>
        public bool SortDescending { get; set; } = false;

        /// <summary>
        /// Registers the column with the parent grid when parameters are set.
        /// </summary>
        protected override void OnParametersSet()
        {
            ParentGrid?.Register(this);
        }

        /// <summary>
        /// Sets the initial sort order after the first render.
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                SortDescending = DefaultSortDescending;
            }
        }

        /// <summary>
        /// Unregisters the column from the parent grid when disposed.
        /// </summary>
        public void Dispose()
        {
            ParentGrid?.Unregister(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Renders the header for the column.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="col"></param>
        public abstract void RenderHeader(RenderTreeBuilder builder, int col);

        /// <summary>
        /// Renders a cell for the given row and column index.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public abstract void RenderCell(RenderTreeBuilder builder, TRow row, int col);
    }

    /// <summary>
    /// A column that uses a RenderFragment to display custom content.
    /// </summary>
    /// <typeparam name="TRow"></typeparam>
    public class ViewColumn<TRow> : ColumnBase<TRow>
    {
        /// <summary>
        /// The content to render for each cell, provided with the current row.
        /// </summary>
        [Parameter, EditorRequired] public RenderFragment<TRow> ChildContent { get; set; } = default!;

        /// <summary>
        /// <inheritdoc cref="ColumnBase{TRow}.RenderHeader(RenderTreeBuilder, int)"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="col"></param>
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

        /// <summary>
        /// <inheritdoc cref="ColumnBase{TRow}.RenderCell(RenderTreeBuilder, TRow, int)"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", CombineClass("rtb-viewcolumn", Class));
            builder.AddAttribute(2, "style", $"grid-column-start: {col}; min-height: fit-content;padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");
            builder.AddContent(3, ChildContent(row));
            builder.CloseElement();
        }
    }

    /// <summary>
    /// A column that displays a value from the row using a provided function.
    /// </summary>
    /// <typeparam name="TRow"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DataColumn<TRow, TValue> : ColumnBase<TRow>
    {
        /// <summary>
        /// Style registry for managing scoped styles.
        /// </summary>
        [Inject] private IStyleRegistry Registry { get; set; } = default!;

        /// <summary>
        /// Function to extract the value from the row to display in the cell.
        /// </summary>
        [Parameter, EditorRequired] public Func<TRow, TValue> ValueFunc { get; set; } = default!;

        private const string CellClass = "rtb-datacolumn";

        /// <summary>
        /// Initializes the component and sets up scoped styles.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var (_, css) = StyleBuilder.Start
                .Set("white-space", "nowrap")
                .Set("overflow", "hidden")
                .Set("text-overflow", "ellipsis")
                .BuildScoped(CellClass);
            await Registry.UpsertScopedAsync(css, CellClass);
        }

        /// <summary>
        /// <inheritdoc cref="ColumnBase{TRow}.RenderHeader(RenderTreeBuilder, int)"/>/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="col"></param>
        public override void RenderHeader(RenderTreeBuilder builder, int col)
        {
            // Add default content if HeadContent is null
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", $"padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");
            builder.AddContent(2, Name);
            builder.CloseElement();
        }

        /// <summary>
        /// <inheritdoc cref="ColumnBase{TRow}.RenderCell(RenderTreeBuilder, TRow, int)"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            var value = ValueFunc(row);
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "role", "cell");
            builder.AddAttribute(2, "class", CombineClass("rtb-datacolumn", Class, CellClass));
            builder.AddAttribute(3, "style", $"grid-column-start: {col}; min-height: fit-content;padding: {string.Join(' ', Spacings.Select(s => s.ToString()))};");
            builder.AddContent(4, value);
            builder.CloseElement();
        }
    }
}
