using Microsoft.Extensions.Logging;

namespace RTB.Blazor.Services.Services;

public interface IDragDropService
{
    object? DraggedItemData { get; }
    void StartDrag<TObject>(TObject? itemData);
    TObject? GetDataOnDrop<TObject>();
}

public class DragDropService(ILogger<DragDropService> logger) : IDragDropService
{
    public object? DraggedItemData { get; private set; }

    public void StartDrag<TObject>(TObject? itemData)
    {
        DraggedItemData = itemData;
    }

    public TObject? GetDataOnDrop<TObject>()
    {
        var data = DraggedItemData;
        DraggedItemData = default;

        if (data is TObject typed)
        {
            return typed;
        }

        throw new InvalidCastException($"DragDropService: Cannot cast object of type {data?.GetType()} to {typeof(TObject)}");
    }
}
