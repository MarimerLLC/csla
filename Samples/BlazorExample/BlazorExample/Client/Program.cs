using System.Threading.Tasks;
using Csla;
using Csla.Blazor.Client.Authentication;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorExample.Client
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("app");

      builder.Services.AddBaseAddressHttpClient();

      builder.Services.AddAuthorizationCore();
      builder.Services.AddOptions();
      builder.Services.AddSingleton<AuthenticationStateProvider, CurrentUserAuthenticationStateProvider>();
      builder.Services.AddSingleton<CurrentUserService>();
      builder.Services.AddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));

      builder.UseCsla((c) =>
        c.DataPortal().DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "http://localhost:57410/api/DataPortal"));

      var host = builder.Build();

      // detect when the authentication state changes, and update Csla ApplicationContext.User
      var authStateProvider = host.Services.GetRequiredService<AuthenticationStateProvider>();
      authStateProvider.AuthenticationStateChanged += AuthStateProvider_AuthenticationStateChanged;

      await host.RunAsync();
    }

    private static void AuthStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
      var cslaPrincipal = new Csla.Security.CslaClaimsPrincipal(task.Result.User);
      ApplicationContext.User = cslaPrincipal;
    }
  }
}
