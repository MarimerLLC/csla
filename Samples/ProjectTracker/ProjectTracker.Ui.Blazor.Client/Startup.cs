using Csla.Configuration;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Ui.Blazor;

namespace ProjectTracker.Ui.Blazor.Client
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCsla().WithBlazorClientSupport();
    }

    public void Configure(IComponentsApplicationBuilder app)
    {
      app.AddComponent<App>("app");

      app.UseCsla();
      app.UseCsla(c => c
        .DataPortal()
          .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "http://localhost:8040/api/dataportal/"));

      //ProjectTracker.Library.RoleList.CacheListAsync();
    }
  }
}
