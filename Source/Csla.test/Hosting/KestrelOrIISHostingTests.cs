using System.Threading.Tasks;
using Csla.TestHelpers.Http;
using Csla.Testing.Business.DataPortal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Hosting;

[TestClass]
public class KestrelOrIISHostingTests {

  private static TestServer _server;
  private static ServiceProvider _clientSideServiceProvider;

  [ClassInitialize]
  public static void Initialize(TestContext context) {
    _ = context;

    var (server, clientServiceProvider) = HttpTestServerFactory.CreateHttpTestServerAndClientServiceProvider();
    _server = server;
    _clientSideServiceProvider = clientServiceProvider;
  }

  [ClassCleanup]
  public static async Task Cleanup() {
    await _clientSideServiceProvider.DisposeAsync();
    _server.Dispose();
  }

  [TestMethod]
  public async Task WhenAllowSynchronousIOIsNotAllowedTheRequestShouldWorkAsExpected() {

    _server.AllowSynchronousIO = false;

    var portal = _clientSideServiceProvider.GetRequiredService<IDataPortal<TestBO>>();
    _ = await portal.CreateAsync();
  }
}