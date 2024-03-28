using Csla.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProjectTracker.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Add render mode detection services
builder.Services.AddTransient<RenderModeProvider>();
builder.Services.AddScoped<ActiveCircuitState>();

builder.Services.AddMemoryCache();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddCsla(o => o
  .AddBlazorWebAssembly(o => o.SyncContextWithServer = true)
  .DataPortal(o => o.ClientSideDataPortal(o => o
    .UseHttpProxy(o => o.DataPortalUrl = "/api/dataportal"))));

await builder.Build().RunAsync();
