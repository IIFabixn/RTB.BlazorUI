using RTB.BlazorUI.Services.Dialog.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace RTB.BlazorUI.Services.Dialog
{
    // Core Contracts
    public enum DialogResultKind { Ok, Cancel, None }

    public sealed record DialogResult(DialogResultKind Kind, object? Data = null)
    {
        public static DialogResult Ok(object? data = null) => new(DialogResultKind.Ok, data);
        public static DialogResult Cancel(object? data = null) => new(DialogResultKind.Cancel, data);
    }

    public interface IDialogReference
    {
        Task<DialogResult> Result { get; }
        void Close(DialogResult result);
        void Close();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDialogService 
    {
        Task<DialogResult> ShowAsync<TDialog>(string title, Dictionary<string, object?>? parameters = null) where TDialog : IComponent;
        void Alert<TDialog>(string title, string? message = null, Dictionary<string, object?>? parameters = null) where TDialog : IComponent;
    }

    /// <summary>
    /// Register this service in the DI container to enable dialogs.
    /// Inject this service into the Application component and add provide the dialog host with a cascarding value of DialogService.
    /// <example>
    /// App.razor:
    ///     <CascadingValue Value = "DialogService" >
    ///         <RouteView RouteData = "@routeData" DefaultLayout="@typeof(MainLayout)" />
    ///         <FocusOnNavigate RouteData = "@routeData" Selector="h1" />
    ///         <DialogProvider />
    ///     </CascadingValue>
    /// Component:
    /// @inject IDialogService DialogService
    /// </example>
    /// </summary>
    public class DialogService : IDialogService
    {
        public event Action<RenderFragment>? OnShow;
        public event Action? OnClose;

        public void Alert<TDialog>(string title, string? message = null, Dictionary<string, object?>? parameters = null) where TDialog : IComponent
        {
            var tcs = new TaskCompletionSource<DialogResult>();

            RenderFragment rf = builder =>
            {
                var seq = 0;
                builder.OpenComponent<AlertHost>(seq++);
                builder.AddAttribute(seq++, nameof(AlertHost.Title), title);
                builder.AddAttribute(seq++, nameof(AlertHost.Message), message);
                builder.AddAttribute(seq++, nameof(AlertHost.ChildContent), (RenderFragment)(b =>
                {
                    var i = 0;
                    b.OpenComponent<TDialog>(i++);

                    // pass parameters through
                    if (parameters is not null)
                    {
                        foreach (var (key, val) in parameters)
                            b.AddAttribute(i, key, val);
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
            };

            OnShow?.Invoke(rf);
        }

        public Task<DialogResult> ShowAsync<TDialog>(
            string title,
            Dictionary<string, object?>? parameters = null)
            where TDialog : IComponent
        {
            var tcs = new TaskCompletionSource<DialogResult>();

            RenderFragment rf = builder =>
            {
                var seq = 0;
                builder.OpenComponent<DialogHost>(seq++);
                builder.AddAttribute(seq++, nameof(DialogHost.Title), title);
                builder.AddAttribute(seq++, nameof(DialogHost.ChildContent), (RenderFragment)(b =>
                {
                    var i = 0;
                    b.OpenComponent<TDialog>(i++);

                    // pass parameters through
                    if (parameters is not null)
                    {
                        foreach (var (key, val) in parameters)
                            b.AddAttribute(i, key, val);
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
            };

            OnShow?.Invoke(rf);
            return tcs.Task;
        }
    }
}
