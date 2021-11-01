using BlazorExample.Client;
using Csla;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAuthorizationCore();
builder.Services.AddOptions();
builder.Services.AddSingleton<AuthenticationStateProvider, CurrentUserAuthenticationStateProvider>();
builder.Services.AddSingleton<CurrentUserService>();

//builder.Services.AddCsla();
var services = builder.Services;

// ApplicationContext
services.TryAddScoped<ApplicationContext>();
services.TryAddScoped(typeof(Csla.Core.IContextManager), typeof(Csla.Core.ApplicationContextManager));

// Data portal services
//services.TryAddTransient(typeof(Csla.Server.IDataPortalServer), typeof(Csla.Server.DataPortal));
//services.TryAddSingleton(typeof(Csla.Server.Dashboard.IDashboard), typeof(Csla.Server.Dashboard.NullDashboard));
//services.TryAddTransient<Csla.Server.DataPortalSelector>();
//services.TryAddTransient<Csla.Server.SimpleDataPortal>();
//services.TryAddTransient<Csla.Server.FactoryDataPortal>();
//services.TryAddTransient<Csla.Server.DataPortalBroker>();

//services.TryAddTransient(typeof(Csla.DataPortalClient.IDataPortalProxy), typeof(Csla.DataPortalClient.HttpProxy));
services.TryAddTransient(typeof(Csla.DataPortalClient.IDataPortalProxy), 
  sp => 
  {
    var applicationContext = sp.GetService<ApplicationContext>();
    var client = sp.GetService<HttpClient>();
    var options = new Csla.Channels.Http.HttpProxyOptions { DataPortalUrl = "/api/DataPortal" };
    return new Csla.Channels.Http.HttpProxy(applicationContext, client, options);
  });

//services.TryAddTransient(typeof(Csla.DataPortalClient.HttpProxyOptions),
//  sp => new Csla.DataPortalClient.HttpProxyOptions { DataPortalUrl = "/api/DataPortal" });

// Data portal API
services.TryAddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
services.TryAddTransient(typeof(IChildDataPortal<>), typeof(DataPortal<>));


builder.UseCsla();

await builder.Build().RunAsync();
