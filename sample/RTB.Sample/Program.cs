using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RTB.Blazor.Extensions;
using RTB.Sample;
using RTB.Sample.Theme;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.UseRTBBlazor(config => config.UseTheme<ISampleTheme>().UseDefaultServices());

await builder.Build().RunAsync();
