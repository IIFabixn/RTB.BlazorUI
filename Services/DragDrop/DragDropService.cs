using Microsoft.Extensions.Logging;

namespace RTB.BlazorUI.Services.DragDrop;

public class DragDropService(ILogger<DragDropService> logger)
{
    public object? DraggedItemData { get; private set; }

    public void StartDrag(object? itemData)
    {
        DraggedItemData = itemData;
        const string message = "Service: Drag Started with {itemData}";
        logger.LogDebug(message, itemData);
    }

    public object? GetDataOnDrop()
    {
        var data = DraggedItemData;
        const string message = "Service: Drop Occurred, returning {data}";
        logger.LogDebug(message, data);
        DraggedItemData = default;
        return data;
    }

    public TObject? GetDataOnDrop<TObject>()
    {
        var data = DraggedItemData;
        const string message = "Service: Drop Occurred, returning {data}";
        logger.LogDebug(message, data);
        DraggedItemData = default;
        return (TObject?)Convert.ChangeType(data, typeof(TObject));
    }
}
