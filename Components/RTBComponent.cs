using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.BusyTracker;
using System.ComponentModel;
using System.Data;

namespace RTB.BlazorUI.Components;

public abstract class RTBComponent : ComponentBase, INotifyPropertyChanged
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> CapturedAttributes { get; set; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Easyily notify the component that it needs to re-render after updating the ref.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetProperty<TValue>(ref TValue key, TValue value)
    {
        key = value;
        PropertyChanged?.Invoke(this, new(nameof(key)));
        StateHasChanged();
    }

    public void StatefulAction(Action action)
    {
        action();
        StateHasChanged();
    }
}
