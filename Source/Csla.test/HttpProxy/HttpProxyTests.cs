using Csla.Channels.Http;
using Csla.Configuration;
using Csla.Test.DataPortalTest;
using Csla.TestHelpers;
using Csla.TestHelpers.Http;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.HttpProxy;

[TestClass]
public class HttpProxyTests
{
  private TestDIContext _testDIContext;
  private TestHttpProxy _systemUnderTest;
  private TestHttpClientHandler _testHttClientHandler;
  private static TestServer _server;
  private static ServiceProvider _clientSideServiceProvider;

  [TestInitialize]
  public void Setup()
  {
    _testDIContext = TestDIContextFactory.CreateDefaultContext();
    var applicationContext = _testDIContext.CreateTestApplicationContext();
    var dataPortalOptions = applicationContext.GetRequiredService<DataPortalOptions>();

    var proxyOptions = new HttpProxyOptions();
    _systemUnderTest = new TestHttpProxy(applicationContext, null, proxyOptions, dataPortalOptions);
    _testHttClientHandler = new TestHttpClientHandler();
    _systemUnderTest.TestHttpClientHandlerToReturn = _testHttClientHandler;

    var (server, clientServiceProvider) = HttpTestServerFactory.CreateHttpTestServerAndClientServiceProvider();
    _server = server;
    _clientSideServiceProvider = clientServiceProvider;
  }

  [TestMethod]
  public async Task GetHttpClientHandler_WhenOverriddenItShouldBeUsedWithinTheCreatedHttpClient()
  {
    using var createdHttpClient = _systemUnderTest.CreateHttpClient();
    _ = await createdHttpClient.GetAsync("http://localhost:1234/weatherForecast");
    _testHttClientHandler.WasCalled.Should().BeTrue();
  }

  [TestMethod]
  public async Task ExecuteAsync_WithPrimitiveObject()
  {
    IDataPortal<SingleCommand> dataPortal = _clientSideServiceProvider.GetRequiredService<IDataPortal<SingleCommand>>();

    var result = await dataPortal.ExecuteAsync(123);
    Assert.IsNotNull(result);
    Assert.AreEqual(123, result.Value);
  }

  [TestMethod]
  public async Task ExecuteAsync()
  {
    IDataPortal<SingleCommand> dataPortal = _clientSideServiceProvider.GetRequiredService<IDataPortal<SingleCommand>>();

    SingleCommand cmd = dataPortal.Create(123);
    var result = await dataPortal.ExecuteAsync(cmd);
    Assert.IsNotNull(result);
    Assert.AreEqual(124, result.Value);
  }

  private class TestHttpClientHandler : HttpClientHandler
  {
    public bool WasCalled { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      WasCalled = true;
      return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
    }
  }

  private class TestHttpProxy : Csla.Channels.Http.HttpProxy
  {
    public TestHttpProxy(ApplicationContext applicationContext, HttpClient httpClient, HttpProxyOptions options, DataPortalOptions dataPortalOptions) 
      : base(applicationContext, httpClient, options, dataPortalOptions)
    {
    }

    public HttpClientHandler TestHttpClientHandlerToReturn { get; set; }

    protected override HttpClientHandler GetHttpClientHandler()
    {
      return TestHttpClientHandlerToReturn;
    }

    public HttpClient CreateHttpClient() => GetHttpClient();
  }
}
