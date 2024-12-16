#if NET6_0_OR_GREATER
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Csla.Configuration;
using Csla.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using System.Threading;

namespace Csla.Blazor.Test
{
  [TestClass]
  public class ApplicationContextTests
  {
    [TestMethod]
    public void CorrectManagerChosen()
    {
      IServiceCollection services = new ServiceCollection();
      services.AddHttpContextAccessor();
      services.AddScoped<Csla.AspNetCore.Blazor.ActiveCircuitState>();
      services.AddScoped(typeof(AuthenticationStateProvider), typeof(AuthenticationStateProviderFake));
      services.AddCsla(c => c
        .AddAspNetCore()
        .AddServerSideBlazor()
      );
      IServiceProvider provider = services.BuildServiceProvider();
      var managers = provider.GetRequiredService<IEnumerable<IContextManager>>();
      Assert.IsTrue(managers.Count() == 2);
      var blazorMgr = (Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor)managers.FirstOrDefault(mgr => mgr is Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor);
      var httpMgr = (Csla.AspNetCore.ApplicationContextManagerHttpContext)managers.FirstOrDefault(mgr => mgr is Csla.AspNetCore.ApplicationContextManagerHttpContext);
      Assert.IsNotNull(blazorMgr);
      Assert.IsTrue(blazorMgr.IsStatefulContext);
      Assert.IsFalse(blazorMgr.IsValid); //because no circuit exists


      Assert.IsNotNull(httpMgr);
      Assert.IsFalse(httpMgr.IsStatefulContext);
      Assert.IsFalse(httpMgr.IsValid); //because no httpcontext exists

      //in this scenario the appliclication context should choose the asynclocal as the 
      var appContext = provider.GetRequiredService<ApplicationContext>();
      Assert.IsNotNull(appContext);
      Assert.IsTrue(appContext.ContextManager is Csla.Core.ApplicationContextManagerAsyncLocal);

    }

  [TestMethod]
  public void UseApplicationContextManagerBlazor()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddHttpContextAccessor();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor());
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = true;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor));
  }

  [TestMethod]
  public void UseAsyncLocalApplicationContextManager()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddHttpContextAccessor();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor());
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = false;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.Core.ApplicationContextManagerAsyncLocal));
  }

  [TestMethod]
  public void UseBlazorApplicationContextManager()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddScoped<IHttpContextAccessor, HttpContextAccessorFake>();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor());
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = true;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor));
  }

  [TestMethod]
  public void UseAspNetCoreOverBlazorApplicationContextManager()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddScoped<IHttpContextAccessor, HttpContextAccessorFake>();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor());
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = false;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.AspNetCore.ApplicationContextManagerHttpContext));
  }

  [TestMethod]
  public void UseAspNetCoreApplicationContextManager()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddScoped<IHttpContextAccessor, HttpContextAccessorFake>();
    services.AddCsla(o => o
      .AddAspNetCore());
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = false;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.AspNetCore.ApplicationContextManagerHttpContext));
  }

  }

  public class AuthenticationStateProviderFake : AuthenticationStateProvider
  {
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
      => Task.FromResult<AuthenticationState>(null);
  }

  public class HttpContextAccessorFake : IHttpContextAccessor
  {
    public HttpContext HttpContext { get; set; } = new HttpContextFake();
  }

  public class HttpContextFake : HttpContext
  {
    public override IFeatureCollection Features => throw new NotImplementedException();

    public override HttpRequest Request => throw new NotImplementedException();

    public override HttpResponse Response => throw new NotImplementedException();

    public override ConnectionInfo Connection => throw new NotImplementedException();

    public override WebSocketManager WebSockets => throw new NotImplementedException();

    public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
    public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Abort() => throw new NotImplementedException();
  }
}
#endif