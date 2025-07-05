using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RTB.Blazor.Theme.Extensions;
using RTB.Blazor.UI.Extensions;
using RTB.Sample;
using RTB.Sample.Theme;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.UseRTBUI();
builder.Services.UseRTBTheme(typeof(ISampleTheme));

await builder.Build().RunAsync();
