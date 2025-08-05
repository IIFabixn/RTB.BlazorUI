using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Services.Components.Dialog;

namespace RTB.Blazor.Services.Services
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
        void Close(object? data);
        void Close();
        void Cancel();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDialogService 
    {
        Task<DialogResult> ShowAsync<TDialog>(Dictionary<string, object?>? contentParameters = null, Dictionary<string, object?>? dialogParameters = null) where TDialog : IComponent;
        Task<DialogResult> ShowAsync(Type dialogType, Dictionary<string, object?>? contentParameters = null, Dictionary<string, object?>? dialogParameters = null);
        void Alert<TDialog>(Dictionary<string, object?>? parameters = null) where TDialog : IComponent;
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

        public Task<DialogResult> ShowAsync(Type dialogType, Dictionary<string, object?>? parameters = null, Dictionary<string, object?>? dialogParameters = null)
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
