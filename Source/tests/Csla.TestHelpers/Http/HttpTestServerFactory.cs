using Csla.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Csla.TestHelpers.Http;
public static class HttpTestServerFactory {

  /// <summary>
  /// Starts a host that serves the test data portal through a <see cref="TestServer"/>
  /// (<c>host.GetTestServer()</c>). Callers stop and dispose the host, which also disposes the server.
  /// </summary>
  public static IHost CreateHttpTestHost() {
    return new HostBuilder()
      .ConfigureWebHost(webHost => webHost
        .UseTestServer()
        .ConfigureServices((IServiceCollection services) =>
        {
          services.AddControllers();
          services.AddHttpContextAccessor();
          services.AddCsla(o => o.AddAspNetCore());
        })
        .Configure(app =>
        {
          app.UseRouting();
          app.UseEndpoints(endpoints => endpoints.MapControllers());
        }))
      .Start();
  }

  public static (IHost Host, TestServer Server, ServiceProvider ClientServiceProvider) CreateHttpTestServerAndClientServiceProvider() {
    var host = CreateHttpTestHost();
    var testServer = host.GetTestServer();

    string controllerName = nameof(TestDataPortalController).Replace("Controller", "");
    var clientServices = new ServiceCollection()
      .AddCsla(
      o => o.DataPortal(
        portalOptions => portalOptions.AddClientSideDataPortal(
          a => a.UseHttpProxy(
            proxy => proxy.DataPortalUrl = $"/api/{controllerName}"
            )
          )
        )
      );
    clientServices.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
    clientServices.AddSingleton(testServer.CreateClient());
    var clientServiceProvider = clientServices.BuildServiceProvider();

    return (host, testServer, clientServiceProvider);
  }
}
