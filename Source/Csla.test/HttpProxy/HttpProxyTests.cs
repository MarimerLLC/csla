using Csla.Channels.Http;
using Csla.Configuration;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.HttpProxy;

[TestClass]
public class HttpProxyTests
{
  private TestDIContext _testDIContext;
  private TestHttpProxy _systemUnderTest;
  private TestHttpClientHandler _testHttClientHandler;

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
  }

  [TestMethod]
  public async Task GetHttpClientHandler_WhenOverriddenItShouldBeUsedWithinTheCreatedHttpClient()
  {
    using var createdHttpClient = _systemUnderTest.CreateHttpClient();
    _ = await createdHttpClient.GetAsync("http://localhost:1234/weatherForecast");
    _testHttClientHandler.WasCalled.Should().BeTrue();
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
