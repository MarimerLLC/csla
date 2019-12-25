using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using Csla.Blazor.Client.Authentication;

namespace ProjectTracker.Ui.Blazor.Client
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthorizationCore();
      services.AddSingleton<AuthenticationStateProvider, CslaAuthenticationStateProvider>();
      services.AddSingleton<CslaUserService>();
      services.AddCsla().WithBlazorClientSupport();
    }

    public void Configure(IComponentsApplicationBuilder app)
    {
      //app.AddComponent<App>("app");
      app.AddComponent<ProjectTracker.Ui.Blazor.App>("app");
      ProjectTracker.Ui.Blazor.App.IsServerSide = false;

      app.UseCsla(c => c
        .DataPortal()
          .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "http://localhost:8040/api/dataportaltext/"));
    }
  }
}
