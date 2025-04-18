namespace RTB.BlazorUI.Services;

public class DragDropService
{
    public object? DraggedItemData { get; private set; }

    public void StartDrag(object? itemData)
    {
        DraggedItemData = itemData;
        Console.WriteLine($"Service: Drag Started with {itemData}");
    }

    public object? GetDataOnDrop()
    {
        var data = DraggedItemData;
        Console.WriteLine($"Service: Drop Occurred, returning {data}");
        // Clear the data after it's been retrieved by the drop zone
        DraggedItemData = null;
        return data;
    }
}
