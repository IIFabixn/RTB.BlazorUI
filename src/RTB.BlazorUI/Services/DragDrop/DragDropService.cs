using Microsoft.Extensions.Logging;

namespace RTB.Blazor.Services.DragDrop;

/// <summary>
/// Provides a lightweight, in-memory mechanism to pass a single drag payload between draggable and droppable
/// components in a Blazor application.
/// </summary>
/// <remarks>
/// Usage:
/// - Call <see cref="StartDrag{TObject}"/> when a drag operation begins.
/// - Call <see cref="GetDataOnDrop{TObject}"/> on the drop target to retrieve and clear the payload.
/// Notes:
/// - The stored payload is cleared on every successful call to <see cref="GetDataOnDrop{TObject}"/>.
/// - Intended DI lifetime is Scoped in Blazor (one instance per circuit/user session).
/// - Not thread-safe. Access from the Blazor UI synchronization context.
/// </remarks>
public interface IDragDropService
{
    /// <summary>
    /// Gets the raw object currently stored as the drag payload.
    /// </summary>
    /// <remarks>
    /// This value is set by <see cref="StartDrag{TObject}"/> and cleared by <see cref="GetDataOnDrop{TObject}"/>.
    /// Prefer using the typed APIs instead of accessing this property directly.
    /// </remarks>
    object? DraggedItemData { get; }

    /// <summary>
    /// Starts a drag operation by storing the provided payload.
    /// </summary>
    /// <typeparam name="TObject">The compile-time type of the payload.</typeparam>
    /// <param name="itemData">The payload to store. May be null to explicitly indicate no data.</param>
    /// <remarks>
    /// This method overwrites any previously stored payload.
    /// </remarks>
    void StartDrag<TObject>(TObject? itemData);

    /// <summary>
    /// Retrieves the drag payload as the requested type and clears the stored payload.
    /// </summary>
    /// <typeparam name="TObject">The expected type for the payload.</typeparam>
    /// <returns>The stored payload cast to <typeparamref name="TObject"/>.</returns>
    /// <exception cref="InvalidCastException">
    /// Thrown when no payload is present or the stored payload cannot be cast to <typeparamref name="TObject"/>.
    /// </exception>
    /// <remarks>
    /// After calling this method, the internal payload is reset to its default (null).
    /// </remarks>
    TObject? GetDataOnDrop<TObject>();
}

/// <summary>
/// Default implementation of <see cref="IDragDropService"/> for Blazor components.
/// </summary>
/// <remarks>
/// Register as a Scoped service in DI for Blazor:
/// services.AddScoped&lt;IDragDropService, DragDropService&gt;();
/// </remarks>
public class DragDropService : IDragDropService
{
    /// <inheritdoc />
    public object? DraggedItemData { get; private set; }

    /// <inheritdoc />
    public void StartDrag<TObject>(TObject? itemData)
    {
        DraggedItemData = itemData;
    }

    /// <inheritdoc />
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
