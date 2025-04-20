using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Components;
using RTB.BlazorUI.Services.Dialog;

namespace RTB.BlazorUI.Dialog.Components
{
    public abstract class DialogBase : RTBComponent, IDialogReference
    {
        [CascadingParameter] public DialogService? DialogService { get; set; }
        [CascadingParameter] public IDialogReference? Dialog { get; set; }

        [Parameter] public virtual string Title { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Backdrop { get; set; } = true;

        private readonly TaskCompletionSource<DialogResult> _tcs = new();

        public Task<DialogResult> Result => _tcs.Task;

        public bool Open => !Result.IsCompleted;

        public void Close(DialogResult result) => _tcs.TrySetResult(result);

        public void Close() => _tcs.TrySetResult(DialogResult.Ok());
    }
}
