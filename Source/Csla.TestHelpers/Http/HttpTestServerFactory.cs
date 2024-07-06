using Csla.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.TestHelpers.Http;
public static class HttpTestServerFactory {

  public static TestServer CreateHttpTestServer() {
    return new TestServer(new WebHostBuilder()
      .ConfigureServices((IServiceCollection services) => {
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddCsla(o => o.AddAspNetCore());
      })
      .Configure(app => {
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
      })
    );
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
    clientServices.AddSingleton(testServer.CreateClient());
    var clientServiceProvider = clientServices.BuildServiceProvider();

    return (testServer, clientServiceProvider);
  }
}
