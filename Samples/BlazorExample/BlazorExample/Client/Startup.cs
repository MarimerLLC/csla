using Csla;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
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

      services.AddAuthorizationCore();
      services.AddSingleton<AuthenticationStateProvider, CurrentUserAuthenticationStateProvider>();
      services.AddSingleton<CurrentUserService>();

    }

    public void Configure(IComponentsApplicationBuilder app)
    {
      app.AddComponent<App>("app");

      // detect when the authentication state changes, and update Csla ApplicationContext.User
      var authStateProvider = app.Services.GetRequiredService<AuthenticationStateProvider>();
      authStateProvider.AuthenticationStateChanged += AuthStateProvider_AuthenticationStateChanged;

      app.UseCsla((c) =>
        c.DataPortal().DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "/api/DataPortal"));
    }

    private void AuthStateProvider_AuthenticationStateChanged(System.Threading.Tasks.Task<AuthenticationState> task)
    {
      Csla.Security.CslaClaimsPrincipal cslaPrincipal = new Csla.Security.CslaClaimsPrincipal(task.Result.User);
      Csla.ApplicationContext.User = cslaPrincipal;
    }
  }
}
