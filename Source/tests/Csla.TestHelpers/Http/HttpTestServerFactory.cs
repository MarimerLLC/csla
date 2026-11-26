using Csla.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Csla.TestHelpers.Http;
public static class HttpTestServerFactory {

  public static TestServer CreateHttpTestServer() {
    // Callers dispose the returned TestServer in their cleanup. The host isn't disposed and lives until the test process ends.
    var host = new HostBuilder()
      .ConfigureWebHost(webHost => webHost
        .UseTestServer()
        .ConfigureServices((IServiceCollection services) => {
          services.AddControllers();
          services.AddHttpContextAccessor();
          services.AddCsla(o => o.AddAspNetCore());
        })
        .Configure(app => {
          app.UseRouting();
          app.UseEndpoints(endpoints => endpoints.MapControllers());
        }))
      .Start();
    return host.GetTestServer();
  }

  public static (TestServer Server, ServiceProvider ClientServiceProvider) CreateHttpTestServerAndClientServiceProvider() {
    var testServer = CreateHttpTestServer();

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

    return (testServer, clientServiceProvider);
  }
}
