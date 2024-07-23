//-----------------------------------------------------------------------
// <copyright file="ContextManagerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context Manager configuration tests</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
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
}


public class AuthenticationStateProviderFake : AuthenticationStateProvider
{
  public override Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    throw new System.NotImplementedException();
  }
}