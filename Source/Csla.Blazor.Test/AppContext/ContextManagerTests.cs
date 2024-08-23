//-----------------------------------------------------------------------
// <copyright file="ContextManagerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context Manager configuration tests</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.Test.AppContext;

/// <summary>
/// Context Manager configuration tests
/// </summary>
[TestClass]
public class ContextManagerTests
{
  [TestMethod]
  public void UseApplicationContextManagerInMemory()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddHttpContextAccessor();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = true));
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = true;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.AspNetCore.Blazor.ApplicationContextManagerInMemory));
  }

  [TestMethod]
  public void UseApplicationContextManagerBlazor()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddHttpContextAccessor();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = false));
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
      .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = false));
    var serviceProvider = services.BuildServiceProvider();

    var activeState = serviceProvider.GetRequiredService<AspNetCore.Blazor.ActiveCircuitState>();
    activeState.CircuitExists = false;

    var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(Csla.Core.ApplicationContextManagerAsyncLocal));
  }

  [TestMethod]
  public void UseAspNetCoreApplicationContextManager()
  {
    var services = new ServiceCollection();
    services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderFake>();
    services.AddScoped<IHttpContextAccessor, HttpContextAccessorFake>();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = false));
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
  {
    throw new System.NotImplementedException();
  }
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
  public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  public override void Abort() => throw new NotImplementedException();
}
