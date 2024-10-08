using Csla.Configuration;
using Marimer.Blazor.RenderMode.WebAssembly;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Add render mode detection services
builder.Services.AddRenderModeDetection();

builder.Services.AddMemoryCache();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddCsla(_ => _
  .AddBlazorWebAssembly(_ => _.SyncContextWithServer = true)
  .DataPortal(_ => _.AddClientSideDataPortal(_ => _
    .UseHttpProxy(_ => _.DataPortalUrl = "/api/dataportal"))));

await builder.Build().RunAsync();
