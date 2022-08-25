using BlazorExample.Client;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAuthorizationCore();
builder.Services.AddOptions();

builder.Services.AddCsla(o => o
  .AddBlazorWebAssembly()
  .DataPortal(dpo => dpo
    .UseHttpProxy(options => options.DataPortalUrl = "/api/DataPortal")));

await builder.Build().RunAsync();
