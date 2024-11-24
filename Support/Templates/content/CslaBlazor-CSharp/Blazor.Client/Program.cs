using Csla.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddCsla(o => o
  .AddBlazorWebAssembly(o => o.SyncContextWithServer = true)
  .Security(o => o.FlowSecurityPrincipalFromClient = true)
  .DataPortal(o => o.AddClientSideDataPortal(o => o
    .UseHttpProxy(o => o.DataPortalUrl = "/api/DataPortal"))));

await builder.Build().RunAsync();
