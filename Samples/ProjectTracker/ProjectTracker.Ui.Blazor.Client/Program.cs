using System;
using System.Net.Http;
using System.Threading.Tasks;
using Csla.Blazor.Client.Authentication;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectTracker.Ui.Blazor.Client
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("app");

      builder.Services.AddSingleton(new HttpClient 
        { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

      builder.Services.AddAuthorizationCore();
      builder.Services.AddOptions();
      builder.Services.AddSingleton
        <AuthenticationStateProvider, CslaAuthenticationStateProvider>();
      builder.Services.AddSingleton<CslaUserService>();
      App.IsServerSide = false;
      builder.UseCsla(c => c
        .DataPortal()
          .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), 
                        "http://localhost:8040/api/dataportaltext/"));

      await builder.Build().RunAsync();
    }
  }
}
