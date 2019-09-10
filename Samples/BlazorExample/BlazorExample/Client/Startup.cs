using Csla;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorExample.Client
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCsla();
      services.AddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
      services.AddTransient(typeof(Csla.Blazor.ViewModel<>), typeof(Csla.Blazor.ViewModel<>));
    }

    public void Configure(IComponentsApplicationBuilder app)
    {
      app.AddComponent<App>("app");

      app.UseCsla((c) =>
        c.DataPortal().DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "/api/DataPortal"));
    }
  }
}
