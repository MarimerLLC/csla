using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Blazor.WebAssembly.State;
using Csla.State;
using Csla.Blazor.WebAssembly.Configuration;
using Csla.Core;
using Csla.Runtime;
using System.Net;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Microsoft.AspNetCore.Components.Authorization;
using NSubstitute;

namespace Csla.Test.State
{
  [TestClass]
  public class SessionManagerTests
  {
    private SessionManager _sessionManager;
    private SessionMessage _sessionValue;

    [TestInitialize]
    public void Initialize()
    {
      var mockServiceProvider = Substitute.For<IServiceProvider>();

      // Mock AuthenticationStateProvider
      var mockAuthStateProvider = Substitute.For<AuthenticationStateProvider>();

      // Mock IServiceProvider
      mockServiceProvider.GetService(typeof(AuthenticationStateProvider)).Returns(mockAuthStateProvider);

      _sessionValue = new SessionMessage
      {
        // Set properties here
        // For example:
        Principal = new Security.CslaClaimsPrincipal() { },
        Session = []
      };

      // Mock ISerializationFormatter
      var mockFormatter = Substitute.For<ISerializationFormatter>();
      mockFormatter.Serialize(Arg.Any<Stream>(), Arg.Any<object>());
      mockFormatter.Deserialize(Arg.Any<Stream>()).Returns(_sessionValue);

      // Mock IServiceProvider
      mockServiceProvider.GetService(typeof(Csla.Serialization.Mobile.MobileFormatter)).Returns(mockFormatter);

      var mockActivator = Substitute.For<Csla.Server.IDataPortalActivator>();
      mockActivator.CreateInstance(Arg.Is<Type>(t => t == typeof(Csla.Serialization.Mobile.MobileFormatter))).Returns(mockFormatter);
      mockActivator.InitializeInstance(Arg.Any<object>());

      // Mock IServiceProvider
      mockServiceProvider.GetService(typeof(Csla.Server.IDataPortalActivator)).Returns(mockActivator);

      // Mock IContextManager
      var mockContextManager = Substitute.For<IContextManager>();
      mockContextManager.IsValid.Returns(true);

      // Mock IContextManagerLocal
      var mockLocalContextManager = Substitute.For<IContextManagerLocal>();

      // Mock IServiceProvider
      mockServiceProvider.GetService(typeof(IRuntimeInfo)).Returns(new RuntimeInfo());

      // Mock IEnumerable<IContextManager>
      var mockContextManagerList = new List<IContextManager> { mockContextManager };

      // Mock ApplicationContextAccessor
      var mockApplicationContextAccessor = Substitute.For<ApplicationContextAccessor>(mockContextManagerList, mockLocalContextManager, mockServiceProvider);

      var _applicationContext = new ApplicationContext(mockApplicationContextAccessor);

      _sessionManager = new SessionManager(_applicationContext, GetHttpClient(_sessionValue, _applicationContext), new BlazorWebAssemblyConfigurationOptions { SyncContextWithServer = true });
    }

    public class TestHttpMessageHandler(SessionMessage session, ApplicationContext _applicationContext) : HttpMessageHandler
    {
      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        cancellationToken.ThrowIfCancellationRequested();
        var response = new HttpResponseMessage()
        {
          StatusCode = HttpStatusCode.OK,
          Content = new StringContent("{\"ResultStatus\":0, \"SessionData\":\"" + Convert.ToBase64String(GetSession(session, _applicationContext)) + "\"}"),
        };
        return Task.FromResult(response);
      }
    }
    private static HttpClient GetHttpClient(SessionMessage session, ApplicationContext _applicationContext)
    {
      var handlerMock = new TestHttpMessageHandler(session,_applicationContext);
      // use real http client with mocked handler here
      var httpClient = new HttpClient(handlerMock)
      {
        BaseAddress = new Uri("http://test.com/"),
      };
      return httpClient;
    }

    private static byte[] GetSession(SessionMessage session, ApplicationContext _applicationContext)
    {
      var info = new MobileFormatter(_applicationContext).SerializeToDTO(session);
      var ms = new MemoryStream();
      new CslaBinaryWriter(_applicationContext).Write(ms, info);
      ms.Position = 0;
      var array = ms.ToArray();
      return array;
    }

    [TestMethod]
    public async Task RetrieveSession_WithTimeoutValue_ShouldNotThrowException()
    {
      var timeout = TimeSpan.FromHours(1);
      var session = await _sessionManager.RetrieveSession(timeout);
      Assert.AreEqual(session, _sessionValue.Session);
    }

    [TestMethod]
    public async Task RetrieveSession_WithZeroTimeout_ShouldThrowTimeoutException()
    {
      var timeout = TimeSpan.Zero;
      await Assert.ThrowsExceptionAsync<TimeoutException>(() => _sessionManager.RetrieveSession(timeout));
    }

    [TestMethod]
    public async Task RetrieveSession_WithValidCancellationToken_ShouldNotThrowException()
    {
      var cts = new CancellationTokenSource();
      var session = await _sessionManager.RetrieveSession(cts.Token);
      Assert.AreEqual(session, _sessionValue.Session);
    }

    [TestMethod]
    public async Task RetrieveSession_WithCancelledCancellationToken_ShouldThrowTaskCanceledException()
    {
      var cts = new CancellationTokenSource();
      cts.Cancel();
      await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => _sessionManager.RetrieveSession(cts.Token));
    }

    [TestMethod]
    public async Task SendSession_WithTimeoutValue_ShouldNotThrowException()
    {
      await _sessionManager.RetrieveSession(TimeSpan.FromHours(1));

      var timeout = TimeSpan.FromHours(1);
      await _sessionManager.SendSession(timeout);
      Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task SendSession_WithZeroTimeout_ShouldThrowTimeoutException()
    {
      await _sessionManager.RetrieveSession(TimeSpan.FromHours(1));

      var timeout = TimeSpan.Zero;
      await Assert.ThrowsExceptionAsync<TimeoutException>(() => _sessionManager.SendSession(timeout));
    }

    [TestMethod]
    public async Task SendSession_WithValidCancellationToken_ShouldNotThrowException()
    {
      await _sessionManager.RetrieveSession(TimeSpan.FromHours(1));

      var cts = new CancellationTokenSource();
      await _sessionManager.SendSession(cts.Token);
      Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task SendSession_WithCancelledCancellationToken_ShouldThrowTaskCanceledException()
    {
      await _sessionManager.RetrieveSession(TimeSpan.FromHours(1));

      var cts = new CancellationTokenSource();
      cts.Cancel();
      await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => _sessionManager.SendSession(cts.Token));
    }


    [TestMethod]
    public async Task  RetrieveCachedSessionSession()
    {
      await _sessionManager.RetrieveSession(TimeSpan.FromHours(1));

      var session = _sessionManager.GetCachedSession();
      Assert.IsNotNull(session);
    }
    [TestMethod]
    public void RetrieveNullCachedSessionSession()
    {
      var session = _sessionManager.GetCachedSession();
      Assert.IsNull(session);
    }
  }
}
