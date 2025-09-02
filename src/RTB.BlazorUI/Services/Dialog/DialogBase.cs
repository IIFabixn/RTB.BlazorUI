using Microsoft.AspNetCore.Components;
using RTB.Blazor.Components;

namespace RTB.Blazor.Services.Dialog
{
    /*
        Plan (pseudocode):
        - Add comprehensive XML documentation to DialogBase:
          - Class summary: purpose, relationship to IDialogReference and IDialogService.
          - Remarks: how the TaskCompletionSource is used; idempotent close; usage pattern.
          - Example: showing and closing a dialog.
        - Add XML docs for:
          - DialogService (cascaded), Dialog (cascaded), ChildContent, Backdrop.
          - Result (awaitable), Open (state).
          - Close overloads: result, data (Ok), parameterless (Cancel); note idempotence and thread-safety.
        - Do not change behavior or public surface, only documentation.
        */

    /// <summary>
    /// Base class for dialog host components.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Provides a common implementation of <see cref="IDialogReference"/> using an internal
    /// <see cref="TaskCompletionSource{TResult}"/> to signal completion when the dialog is closed.
    /// </para>
    /// <para>
    /// Derived components can call one of the <c>Close</c> overloads to complete <see cref="Result"/>.
    /// Calls to <c>Close</c> are idempotent — only the first call wins; subsequent calls are ignored.
    /// </para>
    /// <para>
    /// Typically used in conjunction with <see cref="IDialogService"/> which creates and cascades
    /// an <see cref="IDialogReference"/> to the dialog content.
    /// </para>
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// // Showing a dialog
    /// var result = await Dialogs.ShowAsync<MyDialog>(new() { ["Title"] = "Edit item" });
    /// if (result.Kind is DialogResultKind.Ok) { /* handle success */ }
    ///
    /// // Inside dialog content component
    /// [CascadingParameter] public IDialogReference? Dialog { get; set; }
    ///
    /// void Save(object model) => Dialog?.Close(DialogResult.Ok(model));
    /// void Cancel()           => Dialog?.Close(); // Equivalent to Cancel(null)
    /// ]]>
    /// </example>
    public abstract class DialogBase : RTBComponent, IDialogReference
    {
        /// <summary>
        /// The ambient dialog service, cascaded from the application root.
        /// </summary>
        [CascadingParameter] public IDialogService? DialogService { get; set; }

        /// <summary>
        /// The reference to the active dialog instance, cascaded to child content.
        /// </summary>
        [CascadingParameter] public IDialogReference? Dialog { get; set; }

        /// <summary>
        /// Arbitrary content rendered inside the dialog host (typically the dialog body component).
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Whether an overlay/backdrop is rendered behind the dialog.
        /// </summary>
        [Parameter] public bool Backdrop { get; set; } = true;

        private readonly TaskCompletionSource<DialogResult> _tcs = new();

        /// <summary>
        /// A task that completes when the dialog is closed, yielding the <see cref="DialogResult"/>.
        /// </summary>
        public Task<DialogResult> Result => _tcs.Task;

        /// <summary>
        /// Indicates whether the dialog is still open (<c>true</c>) or has completed (<c>false</c>).
        /// </summary>
        public bool Open => !Result.IsCompleted;

        /// <summary>
        /// Closes the dialog with an explicit <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result to return to the caller awaiting the dialog.</param>
        /// <remarks>
        /// This method is idempotent. Only the first call completes <see cref="Result"/>.
        /// </remarks>
        public void Close(DialogResult result) => _tcs.TrySetResult(result);

        /// <summary>
        /// Closes the dialog with <see cref="DialogResultKind.Ok"/>, optionally returning <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Optional payload to attach to the OK result.</param>
        /// <remarks>
        /// This method is idempotent. Only the first call completes <see cref="Result"/>.
        /// </remarks>
        public void Close(object? data) => _tcs.TrySetResult(DialogResult.Ok(data));

        /// <summary>
        /// Closes the dialog with <see cref="DialogResultKind.Cancel"/>.
        /// </summary>
        /// <remarks>
        /// This method is idempotent. Only the first call completes <see cref="Result"/>.
        /// </remarks>
        public void Close() => _tcs.TrySetResult(DialogResult.Cancel());
    }
}
