using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Csla.Blazor.WebAssembly.State;
using Csla.State;
using Moq;
using System.Net.Http;
using Csla.Blazor.WebAssembly.Configuration;
using Csla.Core;
using Csla.Runtime;
using Moq.Protected;
using System.Net;
using Csla.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Csla.Serialization.Mobile;
using Microsoft.AspNetCore.Components.Authorization;

namespace Csla.Test.State
{
  [TestClass]
  public class SessionManagerTests
  {
    private SessionManager _SessionManager;
    private SessionMessage SessionValue;

    [TestInitialize]
    public async Task Initialize()
    {
      var mockServiceProvider = new Mock<IServiceProvider>();

      // Mock AuthenticationStateProvider
      var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();

      // Mock IServiceProvider
      mockServiceProvider.Setup(x => x.GetService(typeof(AuthenticationStateProvider))).Returns(mockAuthStateProvider.Object);

      SessionValue = new SessionMessage
      {
        // Set properties here
        // For example:
        Principal = new Security.CslaClaimsPrincipal() { },
        Session = []
      };

      // Mock ISerializationFormatter
      var mockFormatter = new Mock<ISerializationFormatter>();
      mockFormatter.Setup(x => x.Serialize(It.IsAny<Stream>(), It.IsAny<object>()));
      mockFormatter.Setup(x => x.Deserialize(It.IsAny<Stream>())).Returns(SessionValue);

      // Mock IServiceProvider
      mockServiceProvider.Setup(x => x.GetService(typeof(Csla.Serialization.Mobile.MobileFormatter))).Returns(mockFormatter.Object);

      var mockActivator = new Mock<Csla.Server.IDataPortalActivator>();
      mockActivator.Setup(x => x.CreateInstance(It.Is<Type>(t => t == typeof(Csla.Serialization.Mobile.MobileFormatter)))).Returns(mockFormatter.Object);
      mockActivator.Setup(x => x.InitializeInstance(It.IsAny<object>()));

      // Mock IServiceProvider
      mockServiceProvider.Setup(x => x.GetService(typeof(Csla.Server.IDataPortalActivator))).Returns(mockActivator.Object);

      // Mock IContextManager
      var mockContextManager = new Mock<IContextManager>();
      mockContextManager.Setup(x => x.IsValid).Returns(true);

      // Mock IContextManagerLocal
      var mockLocalContextManager = new Mock<IContextManagerLocal>();

      // Mock IServiceProvider
      mockServiceProvider.Setup(x => x.GetService(typeof(IRuntimeInfo))).Returns(new RuntimeInfo());

      // Mock IEnumerable<IContextManager>
      var mockContextManagerList = new List<IContextManager> { mockContextManager.Object };

      // Mock ApplicationContextAccessor
      var mockApplicationContextAccessor = new Mock<ApplicationContextAccessor>(mockContextManagerList, mockLocalContextManager.Object, mockServiceProvider.Object);

      var _applicationContext = new ApplicationContext(mockApplicationContextAccessor.Object);


      _SessionManager = new SessionManager(_applicationContext, GetHttpClient(SessionValue, _applicationContext), new BlazorWebAssemblyConfigurationOptions { SyncContextWithServer = true });
      await _SessionManager.RetrieveSession(TimeSpan.FromHours(1));
    }

    private static HttpClient GetHttpClient(SessionMessage session, ApplicationContext _applicationContext)
    {
      var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
      handlerMock
         .Protected()
         // Setup the PROTECTED method to mock
         .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
         )
         // prepare the expected response of the mocked http call
         .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
         {
           if (cancellationToken.IsCancellationRequested)
           {
             throw new OperationCanceledException(cancellationToken);
           }
           else
           {
             return new HttpResponseMessage()
             {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("{\"ResultStatus\":0, \"SessionData\":\"" + Convert.ToBase64String(GetSession(session, _applicationContext)) + "\"}"),
             };
           }
         })
         .Verifiable();

      // use real http client with mocked handler here
      var httpClient = new HttpClient(handlerMock.Object)
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
      var session = await _SessionManager.RetrieveSession(timeout);
      Assert.AreEqual(session, SessionValue.Session);
    }

    [TestMethod]
    public async Task RetrieveSession_WithZeroTimeout_ShouldThrowTimeoutException()
    {
      var timeout = TimeSpan.Zero;
      await Assert.ThrowsExceptionAsync<TimeoutException>(() => _SessionManager.RetrieveSession(timeout));
    }

    [TestMethod]
    public async Task RetrieveSession_WithValidCancellationToken_ShouldNotThrowException()
    {
      var cts = new CancellationTokenSource();
      var session = await _SessionManager.RetrieveSession(cts.Token);
      Assert.AreEqual(session, SessionValue.Session);
    }

    [TestMethod]
    public async Task RetrieveSession_WithCancelledCancellationToken_ShouldThrowTaskCanceledException()
    {
      var cts = new CancellationTokenSource();
      cts.Cancel();
      await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => _SessionManager.RetrieveSession(cts.Token));
    }

    [TestMethod]
    public async Task SendSession_WithTimeoutValue_ShouldNotThrowException()
    {
      var timeout = TimeSpan.FromHours(1);
      await _SessionManager.SendSession(timeout);
      Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task SendSession_WithZeroTimeout_ShouldThrowTimeoutException()
    {
      var timeout = TimeSpan.Zero;
      await Assert.ThrowsExceptionAsync<TimeoutException>(() => _SessionManager.SendSession(timeout));
    }

    [TestMethod]
    public async Task SendSession_WithValidCancellationToken_ShouldNotThrowException()
    {
      var cts = new CancellationTokenSource();
      await _SessionManager.SendSession(cts.Token);
      Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task SendSession_WithCancelledCancellationToken_ShouldThrowTaskCanceledException()
    {
      var cts = new CancellationTokenSource();
      cts.Cancel();
      await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => _SessionManager.SendSession(cts.Token));
    }


    [TestMethod]
    public void  RetrieveCAchedSessionSession()
    {
      _SessionManager.GetCachedSession();
    }
  }
}
