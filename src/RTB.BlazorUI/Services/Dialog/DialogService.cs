using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RTB.Blazor.Services.Dialog
{
    /// <summary>
    /// Represents the outcome of a dialog interaction.
    /// </summary>
    public enum DialogResultKind
    {
        /// <summary>
        /// The dialog completed successfully (e.g., primary action).
        /// </summary>
        Ok,

        /// <summary>
        /// The dialog was canceled (e.g., secondary action, escape, close button).
        /// </summary>
        Cancel,

        /// <summary>
        /// The dialog closed without an explicit result. Typically used as a sentinel.
        /// </summary>
        None
    }

    /// <summary>
    /// Immutable result returned by a dialog when it closes.
    /// </summary>
    /// <param name="Kind">The outcome of the dialog.</param>
    /// <param name="Data">Optional payload returned by the dialog (e.g., form data).</param>
    /// <remarks>
    /// Use <see cref="Ok(object?)"/> or <see cref="Cancel(object?)"/> to create a result with a conventional outcome.
    /// </remarks>
    /// <example>
    /// In a dialog component:
    /// <![CDATA[
    /// [CascadingParameter] public IDialogReference? Dialog { get; set; }
    ///
    /// void Save()
    ///     => Dialog?.Close(DialogResult.Ok(new { Name, Email }));
    ///
    /// void Dismiss()
    ///     => Dialog?.Close(); // Equivalent to Cancel()
    /// ]]>
    /// </example>
    public sealed record DialogResult(DialogResultKind Kind, object? Data = null)
    {
        /// <summary>
        /// Creates a successful result with an optional payload.
        /// </summary>
        public static DialogResult Ok(object? data = null) => new(DialogResultKind.Ok, data);

        /// <summary>
        /// Creates a canceled result with an optional payload (e.g., reason).
        /// </summary>
        public static DialogResult Cancel(object? data = null) => new(DialogResultKind.Cancel, data);
    }

    /// <summary>
    /// A reference to an active dialog instance. Allows awaiting completion and closing it programmatically.
    /// </summary>
    public interface IDialogReference
    {
        /// <summary>
        /// A task that completes when the dialog is closed, yielding the <see cref="DialogResult"/>.
        /// </summary>
        Task<DialogResult> Result { get; }

        /// <summary>
        /// Closes the dialog with an explicit <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result to return to the caller awaiting the dialog.</param>
        void Close(DialogResult result);

        /// <summary>
        /// Closes the dialog, returning <see cref="DialogResultKind.Ok"/> and the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Optional payload to attach to an OK result.</param>
        void Close(object? data);

        /// <summary>
        /// Closes the dialog, returning <see cref="DialogResultKind.Cancel"/>.
        /// </summary>
        void Close();
    }

    /// <summary>
    /// Service surface for presenting dialogs.
    /// </summary>
    /// <remarks>
    /// Registration (Program.cs):
    /// <code>
    /// builder.Services.AddScoped&lt;IDialogService, DialogService&gt;();
    /// </code>
    /// App.razor:
    /// <code>
    /// @using RTB.Blazor.Services.Dialog
    /// @inject IDialogService DialogService
    /// 
    /// &lt;CascadingValue Value="DialogService"&gt;
    ///     &lt;Router AppAssembly="typeof(App).Assembly"&gt;
    ///         &lt;Found Context="routeData"&gt;
    ///             &lt;RouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)" /&gt;
    ///         &lt;/Found&gt;
    ///         &lt;NotFound&gt;...&lt;/NotFound&gt;
    ///     &lt;/Router&gt;
    ///     &lt;DialogProvider /&gt;
    /// &lt;/CascadingValue&gt;
    /// </code>
    /// Usage in a component:
    /// <code>
    /// @inject IDialogService Dialogs
    /// 
    /// var result = await Dialogs.ShowAsync&lt;MyDialog&gt;(new()
    /// {
    ///     ["Title"] = "Edit item",
    ///     ["Model"] = item
    /// });
    /// if (result.Kind is DialogResultKind.Ok) { /* handle success */ }
    /// </code>
    /// </remarks>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a dialog rendering the specified <typeparamref name="TDialog"/> component.
        /// </summary>
        /// <typeparam name="TDialog">The dialog content component type.</typeparam>
        /// <param name="contentParameters">Attributes passed to the dialog content component.</param>
        /// <param name="dialogParameters">Attributes passed to the dialog host (e.g., size, modality).</param>
        /// <returns>A task that completes with the <see cref="DialogResult"/> when the dialog closes.</returns>
        Task<DialogResult> ShowAsync<TDialog>(
            Dictionary<string, object?>? contentParameters = null,
            Dictionary<string, object?>? dialogParameters = null
        ) where TDialog : IComponent;

        /// <summary>
        /// Shows a dialog rendering the specified component <paramref name="dialogType"/>.
        /// </summary>
        /// <param name="dialogType">The dialog content component type (must implement <see cref="IComponent"/>).</param>
        /// <param name="contentParameters">Attributes passed to the dialog content component.</param>
        /// <param name="dialogParameters">Attributes passed to the dialog host (e.g., size, modality).</param>
        /// <returns>A task that completes with the <see cref="DialogResult"/> when the dialog closes.</returns>
        Task<DialogResult> ShowAsync(
            Type dialogType,
            Dictionary<string, object?>? contentParameters = null,
            Dictionary<string, object?>? dialogParameters = null
        );

        /// <summary>
        /// Presents a transient alert using the specified <typeparamref name="TDialog"/> content.
        /// </summary>
        /// <param name="parameters">Attributes passed to the alert content component.</param>
        /// <remarks>
        /// This is fire-and-forget and does not return a <see cref="Task"/>. Use for notifications/toasts.
        /// </remarks>
        void Alert<TDialog>(Dictionary<string, object?>? parameters = null) where TDialog : IComponent;
    }

    /// <summary>
    /// Default <see cref="IDialogService"/> implementation using <see cref="RenderFragment"/> composition.
    /// </summary>
    /// <remarks>
    /// - Raises <see cref="OnShow"/> with a <see cref="RenderFragment"/> to render a dialog host (<c>DialogHost</c> or <c>AlertHost</c>).
    /// - Completes the awaiting task when the host signals completion through <see cref="IDialogReference.Result"/>.
    /// - Invokes <see cref="OnClose"/> when the dialog completes.
    /// 
    /// Ensure a provider component (e.g., <c>DialogProvider</c>) subscribes to <see cref="OnShow"/> and renders the supplied fragment.
    /// </remarks>
    /// <seealso cref="IDialogService"/>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Raised when a dialog should be rendered. The provided fragment renders the host and content.
        /// </summary>
        public event Action<RenderFragment>? OnShow;

        /// <summary>
        /// Raised when a dialog has closed (after the result is set).
        /// </summary>
        public event Action? OnClose;

        /// <summary>
        /// Presents a transient alert using the specified <typeparamref name="TDialog"/> content.
        /// </summary>
        /// <typeparam name="TDialog">The alert content component type.</typeparam>
        /// <param name="parameters">Attributes passed to the alert content component.</param>
        /// <remarks>
        /// - Renders an <c>AlertHost</c> and nests <typeparamref name="TDialog"/> inside it.
        /// - Does not return a task; suitable for ephemeral notifications.
        /// - When the alert completes, <see cref="OnClose"/> is invoked.
        /// </remarks>
        public void Alert<TDialog>(Dictionary<string, object?>? parameters = null) where TDialog : IComponent
        {
            var tcs = new TaskCompletionSource<DialogResult>();

            void rf(RenderTreeBuilder builder)
            {
                var seq = 0;
                builder.OpenComponent<AlertHost>(seq++);
                builder.AddAttribute(seq++, nameof(AlertHost.ChildContent), (RenderFragment)(b =>
                {
                    var i = 0;
                    b.OpenComponent<TDialog>(i++);
                    if (parameters is not null and { Count: > 0 })
                    {
                        b.AddMultipleAttributes(i++, parameters!);
                    }
                    b.CloseComponent();
                }));

                builder.AddComponentReferenceCapture(seq++, obj =>
                {
                    if (obj is IDialogReference dr)
                    {
                        dr.Result.ContinueWith(task =>
                        {
                            tcs.TrySetResult(task.Result);
                            OnClose?.Invoke();
                        }, TaskScheduler.Current);
                    }
                });

                builder.CloseComponent();
            }

            OnShow?.Invoke(rf);
        }

        /// <summary>
        /// Shows a dialog rendering the specified component <paramref name="dialogType"/>.
        /// </summary>
        /// <param name="dialogType">The dialog content component type (must implement <see cref="IComponent"/>).</param>
        /// <param name="parameters">Attributes passed to the dialog content component.</param>
        /// <param name="dialogParameters">Attributes passed to the dialog host (e.g., size, modality).</param>
        /// <returns>A task that completes with the <see cref="DialogResult"/> when the dialog closes.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="dialogType"/> does not implement <see cref="IComponent"/>.</exception>
        public Task<DialogResult> ShowAsync(
            Type dialogType,
            Dictionary<string, object?>? parameters = null,
            Dictionary<string, object?>? dialogParameters = null)
        {
            if (!typeof(IComponent).IsAssignableFrom(dialogType))
                throw new ArgumentException("Type must implement IComponent", nameof(dialogType));

            var tcs = new TaskCompletionSource<DialogResult>();

            void rf(RenderTreeBuilder builder)
            {
                var seq = 0;
                builder.OpenComponent<DialogHost>(seq++);
                if (dialogParameters is not null and { Count: > 0 })
                {
                    builder.AddMultipleAttributes(seq++, dialogParameters!);
                }

                builder.AddAttribute(seq++, nameof(DialogHost.ChildContent), (RenderFragment)(b =>
                {
                    var i = 0;
                    b.OpenComponent(i++, dialogType);
                    if (parameters is not null and { Count: > 0 })
                    {
                        b.AddMultipleAttributes(i++, parameters!);
                    }
                    b.CloseComponent();
                }));

                builder.AddComponentReferenceCapture(seq++, obj =>
                {
                    if (obj is IDialogReference dr)
                    {
                        dr.Result.ContinueWith(task =>
                        {
                            tcs.TrySetResult(task.Result);
                            OnClose?.Invoke();
                        }, TaskScheduler.Current);
                    }
                });

                builder.CloseComponent();
            }

            OnShow?.Invoke(rf);
            return tcs.Task;
        }

        /// <summary>
        /// Shows a dialog rendering the specified <typeparamref name="TDialog"/> component.
        /// </summary>
        /// <typeparam name="TDialog">The dialog content component type.</typeparam>
        /// <param name="parameters">Attributes passed to the dialog content component.</param>
        /// <param name="dialogParameters">Attributes passed to the dialog host (e.g., size, modality).</param>
        /// <returns>A task that completes with the <see cref="DialogResult"/> when the dialog closes.</returns>
        public Task<DialogResult> ShowAsync<TDialog>(
            Dictionary<string, object?>? parameters = null,
            Dictionary<string, object?>? dialogParameters = null
        )
            where TDialog : IComponent
        {
            return ShowAsync(typeof(TDialog), parameters, dialogParameters);
        }
    }
}
