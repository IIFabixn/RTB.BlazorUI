using Microsoft.AspNetCore.Components;
using RTB.Blazor.Core;
using RTB.Blazor.Services.Services;

namespace RTB.Blazor.Services.Components.Dialog
{
    public abstract class DialogBase : RTBComponent, IDialogReference
    {
        [CascadingParameter] public IDialogService? DialogService { get; set; }
        [CascadingParameter] public IDialogReference? Dialog { get; set; }

        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter] public bool Backdrop { get; set; } = true;

        private readonly TaskCompletionSource<DialogResult> _tcs = new();

        public Task<DialogResult> Result => _tcs.Task;

        public bool Open => !Result.IsCompleted;

        public void Close(DialogResult result) => _tcs.TrySetResult(result);
        public void Close(object? data) => _tcs.TrySetResult(DialogResult.Ok(data));
        public void Close() => _tcs.TrySetResult(DialogResult.Cancel());
    }
}
