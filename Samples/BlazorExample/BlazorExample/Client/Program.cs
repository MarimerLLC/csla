using BlazorExample.Client;
using BlazorExample.Shared;
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
builder.Services.AddScoped<AuthenticationStateProvider, CurrentUserAuthenticationStateProvider>();
builder.Services.AddScoped<CurrentUserService>();

builder.Services.AddCsla(o=>o
  .WithBlazorWebAssembly()
  .DataPortal()
    .UseHttpProxy(options => options.DataPortalUrl = "/api/DataPortal"));

builder.UseCsla();

await builder.Build().RunAsync();
