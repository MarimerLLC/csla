//-----------------------------------------------------------------------
// <copyright file="SessionManagerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Blazor.WebAssembly.State;
using Csla.State;
using Csla.Blazor.WebAssembly.Configuration;
using System.Net;
using Csla.Serialization.Mobile;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;

namespace Csla.Test.State;

[TestClass]
public class SessionManagerTests
{
  private SessionManager _sessionManager;
  private SessionMessage _sessionValue;

  [TestInitialize]
  public void Initialize()
  {
    var services = new ServiceCollection();
    services.AddCsla(o => o.AddBlazorWebAssembly());
    var provider = services.BuildServiceProvider();
    var _applicationContext = provider.GetRequiredService<ApplicationContext>();

    _sessionValue = new SessionMessage
    {
      Principal = new ClaimsPrincipal() { },
      Session = []
    };

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
    return new MobileFormatter(_applicationContext).SerializeToByteArray(session);
  }

  [TestMethod]
  public async Task RetrieveSession_WithTimeoutValue_ShouldNotThrowException()
  {
    var timeout = TimeSpan.FromHours(1);
    var session = await _sessionManager.RetrieveSession(timeout);
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
