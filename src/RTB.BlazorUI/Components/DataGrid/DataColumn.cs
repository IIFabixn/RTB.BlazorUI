using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Core;
using RTB.Blazor.UI.Interfaces;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styled.Services;

namespace RTB.Blazor.UI.Components.DataGrid
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
            //builder.OpenElement(0, "div");
            builder.AddContent(0, Name);
            //builder.CloseElement();
        }

        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            var seq = 0;
            builder.OpenComponent<Styled.Styled>(seq++);
            builder.AddComponentParameter(seq++, nameof(Styled.Styled.ChildContent), (RenderFragment<string>)((className) => _builder =>
            {
                _builder.OpenElement(0, "div");
                _builder.AddAttribute(1, "class", CombineClass("rtb-viewcolumn", className, Class));
                _builder.AddContent(2,ChildContent(row));
                _builder.CloseElement();
            }));
            builder.CloseComponent();
        }
    }

    public class DataColumn<TRow, TValue> : ColumnBase<TRow>
    {
        [Inject] private IStyleRegistry Registry { get; set; } = default!;

        [Parameter, EditorRequired] public Func<TRow, TValue> ValueFunc { get; set; } = default!;

        private string? CellClass;

        protected override async Task OnInitializedAsync()
        {
            var style = StyleBuilder.Start
                .Append("white-space", "nowrap")
                .Append("overflow", "hidden")
                .Append("text-overflow", "ellipsis")
                .Build();

            CellClass ??= Registry.GetOrCreate(style);
            await Registry.InjectInto(style, CellClass);
        }

        public override void RenderHeader(RenderTreeBuilder builder, int col)
        {
            // Add default content if HeadContent is null
            builder.OpenElement(0, "div");
            builder.AddContent(0, Name);
            builder.CloseElement();
        }

        public override void RenderCell(RenderTreeBuilder builder, TRow row, int col)
        {
            var seq = 0;
            var value = ValueFunc(row);
            builder.OpenComponent<Styled.Styled>(seq++);
            builder.AddComponentParameter(seq++, nameof(Styled.Styled.ChildContent), (RenderFragment<string>)(classname => _builder => {
                var seq = 0;
                _builder.OpenElement(seq++, "div");
                _builder.AddAttribute(seq++, "role", "cell");
                _builder.AddAttribute(seq++, "class", CombineClass("rtb-datacolumn", CellClass, Class));
                _builder.AddAttribute(seq++, "style", $"grid-column-start: {col}; min-height: fit-content;");
                _builder.AddContent(seq++, value);
                _builder.CloseElement();
            }));

            builder.CloseComponent();
        }
    }
}
