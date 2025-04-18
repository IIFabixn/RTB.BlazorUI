using Microsoft.Extensions.Logging;

namespace RTB.BlazorUI.Services.DragDrop;

public class DragDropService(ILogger<DragDropService> logger)
{
    public object? DraggedItemData { get; private set; }

    public void StartDrag(object? itemData)
    {
        DraggedItemData = itemData;
        logger.LogDebug($"Service: Drag Started with {itemData}");
    }

    public object? GetDataOnDrop()
    {
        var data = DraggedItemData;
        logger.LogDebug($"Service: Drop Occurred, returning {data}");
        DraggedItemData = default;
        return data;
    }

    public TObject? GetDataOnDrop<TObject>()
    {
        var data = DraggedItemData;
        logger.LogDebug($"Service: Drop Occurred, returning {data}");
        DraggedItemData = default;
        return (TObject?)Convert.ChangeType(data, typeof(TObject));
    }
}
