using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorApp.Client
{
  public class Program
  {
    static void Main(string[] args)
    {
      Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
      Csla.DataPortalClient.HttpProxy.UseTextSerialization = true;
      Csla.DataPortalClient.HttpProxy.DefaultUrl = "/api/DataPortal";
      Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();

      var serviceProvider = new BrowserServiceProvider(services =>
      {
              // Add any custom services here
            });

      new BrowserRenderer(serviceProvider).AddComponent<App>("app");
    }
  }
}
