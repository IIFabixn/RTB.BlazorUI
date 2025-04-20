using Microsoft.Extensions.Logging;

namespace RTB.BlazorUI.Services.DragDrop;

public class DragDropService(ILogger<DragDropService> logger)
{
    public object? DraggedItemData { get; private set; }

    public void StartDrag<TObject>(TObject? itemData)
    {
        DraggedItemData = itemData;
        const string message = "Service: Drag Started with {itemData}";
        logger.LogDebug(message, itemData);
    }

    public TObject? GetDataOnDrop<TObject>()
    {
        var data = DraggedItemData;
        const string message = "Service: Drop Occurred, returning {data}";
        logger.LogDebug(message, data);
        DraggedItemData = default;

        if (data is TObject typed)
        {
            return typed;
        }

        throw new InvalidCastException($"DragDropService: Cannot cast object of type {data?.GetType()} to {typeof(TObject)}");
    }
}
