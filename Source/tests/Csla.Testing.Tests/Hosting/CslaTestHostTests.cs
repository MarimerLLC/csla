//-----------------------------------------------------------------------
// <copyright file="CslaTestHostTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the CslaTestHost self-contained test host</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Configuration;
using Csla.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Testing.Tests.Hosting
{
  /// <summary>
  /// Tests for <see cref="CslaTestHost"/>, the self-contained host a test uses
  /// when it does not need to own the service collection.
  /// </summary>
  [TestClass]
  public class CslaTestHostTests
  {
    [TestMethod]
    public void CreateProducesAUsableApplicationContext()
    {
      using var host = CslaTestHost.Create();

      host.ApplicationContext.Should().NotBeNull();
      host.Services.Should().NotBeNull();
    }

    [TestMethod]
    public void CreateBuildsOnAddCslaTesting()
    {
      using var host = CslaTestHost.Create();

      host.Services.GetRequiredService<IHostEnvironment>().Should().BeOfType<CslaTestHostEnvironment>();
      host.Services.GetRequiredService<IContextManager>().Should().BeOfType<CslaTestContextManager>();
    }

    [TestMethod]
    public void TheDefaultUserIsAuthenticatedWithNoRoles()
    {
      using var host = CslaTestHost.Create();

      var user = host.ApplicationContext.User;

      user.Identity.IsAuthenticated.Should().BeTrue();
      user.Identity.Name.Should().Be("TestUser");
      user.IsInRole("Admin").Should().BeFalse();
    }

    [TestMethod]
    public void AsUserSetsThePrincipalForTheTest()
    {
      using var host = CslaTestHost.Create(t => t.AsUser("alice", "Admin"));

      host.ApplicationContext.User.Identity.Name.Should().Be("alice");
      host.ApplicationContext.User.IsInRole("Admin").Should().BeTrue();
    }

    [TestMethod]
    public void AsUnauthenticatedProducesAnAnonymousUser()
    {
      using var host = CslaTestHost.Create(t => t.AsUnauthenticated());

      host.ApplicationContext.User.Identity.IsAuthenticated.Should().BeFalse();
    }

    [TestMethod]
    public void AsPrincipalUsesTheSuppliedPrincipal()
    {
      var identity = new ClaimsIdentity("custom", ClaimTypes.Name, ClaimTypes.Role);
      identity.AddClaim(new Claim(ClaimTypes.Name, "bob"));
      var principal = new ClaimsPrincipal(identity);

      using var host = CslaTestHost.Create(t => t.AsPrincipal(principal));

      host.ApplicationContext.User.Should().BeSameAs(principal);
    }

    [TestMethod]
    public async Task GetDataPortalReturnsAWorkingDataPortal()
    {
      using var host = CslaTestHost.Create(t => t
        .AsUser("alice", "Admin")
        .ConfigureServices(s => s.AddScoped<INameSource, FakeNameSource>()));

      var portal = host.GetDataPortal<HostedRoot>();
      var root = await portal.FetchAsync(42);

      root.Id.Should().Be(42);
      root.Name.Should().Be("Name 42", "the fake registered for the test must be injected");
      root.UserName.Should().Be("alice", "the configured principal must reach the data portal operation");
    }

    [TestMethod]
    public async Task GetChildDataPortalReturnsAWorkingChildDataPortal()
    {
      using var host = CslaTestHost.Create();

      var portal = host.GetChildDataPortal<HostedChild>();
      var child = await portal.FetchChildAsync(7);

      child.Id.Should().Be(7);
    }

    [TestMethod]
    public void ConfigureCslaReachesTheCslaOptions()
    {
      using var host = CslaTestHost.Create(t => t
        .ConfigureCsla(o => o.DataPortal(dp => dp.AddServerSideDataPortal(
          so => so.RegisterDashboard<Server.Dashboard.Dashboard>()))));

      host.Services.GetRequiredService<Server.Dashboard.IDashboard>()
        .Should().BeOfType<Server.Dashboard.Dashboard>();
    }

    [TestMethod]
    public void TheDashboardIsLeftAtTheFrameworkDefault()
    {
      // the helper this replaces silently upgraded every test to the real dashboard
      using var host = CslaTestHost.Create();

      host.Services.GetRequiredService<Server.Dashboard.IDashboard>()
        .Should().BeOfType<Server.Dashboard.NullDashboard>();
    }

    [TestMethod]
    public void ConfigureCslaComposesRepeatedCalls()
    {
      var first = false;
      var second = false;

      using var host = CslaTestHost.Create(t => t
        .ConfigureCsla(_ => first = true)
        .ConfigureCsla(_ => second = true));

      first.Should().BeTrue();
      second.Should().BeTrue();
    }

    [TestMethod]
    public void ConfigureServicesComposesRepeatedCalls()
    {
      using var host = CslaTestHost.Create(t => t
        .ConfigureServices(s => s.AddSingleton<INameSource, FakeNameSource>())
        .ConfigureServices(s => s.AddSingleton<DisposalSentinel>()));

      host.Services.GetRequiredService<INameSource>().Should().NotBeNull();
      host.Services.GetRequiredService<DisposalSentinel>().Should().NotBeNull();
    }

    [TestMethod]
    public void CallerRegistrationsWinOverTheHostsOwn()
    {
      // ConfigureServices is applied first, so the caller wins the TryAdd race
      using var host = CslaTestHost.Create(t => t
        .ConfigureServices(s => s.AddSingleton<IHostEnvironment>(
          new CslaTestHostEnvironment { ApplicationName = "Mine" })));

      host.Services.GetRequiredService<IHostEnvironment>().ApplicationName.Should().Be("Mine");
    }

    [TestMethod]
    public void CallerRegistrationsWinOverServicesCslaRegistersWithoutTryAdd()
    {
      // AddCsla registers some services with a plain AddScoped rather than TryAdd,
      // so the caller's registration has to come last to win: a service resolves to
      // the last registration for its type
      using var host = CslaTestHost.Create(t => t
        .ConfigureServices(s => s.AddScoped<Csla.Rules.IUnhandledAsyncRuleExceptionHandler, FakeUnhandledAsyncRuleExceptionHandler>()));

      host.Services.GetRequiredService<Csla.Rules.IUnhandledAsyncRuleExceptionHandler>()
        .Should().BeOfType<FakeUnhandledAsyncRuleExceptionHandler>();
    }

    [TestMethod]
    public void ACallerSuppliedContextManagerStillGetsTheConfiguredPrincipal()
    {
      // this is the shape Csla.TestHelpers uses; the principal must still apply
      using var host = CslaTestHost.Create(t => t
        .AsUser("alice", "Admin")
        .ConfigureServices(s => s.AddSingleton<IContextManager, CustomContextManager>()));

      host.Services.GetRequiredService<IContextManager>().Should().BeOfType<CustomContextManager>();
      host.ApplicationContext.User.Identity.Name.Should().Be("alice");
    }

    [TestMethod]
    public void CreateScopeReturnsAUsableScope()
    {
      using var host = CslaTestHost.Create();

      using var scope = host.CreateScope();

      scope.ServiceProvider.GetRequiredService<ApplicationContext>().Should().NotBeNull();
    }

    [TestMethod]
    public void DisposingTheHostDisposesTheServiceProvider()
    {
      var host = CslaTestHost.Create(t => t.ConfigureServices(s => s.AddSingleton<DisposalSentinel>()));
      var sentinel = host.Services.GetRequiredService<DisposalSentinel>();

      host.Dispose();

      sentinel.IsDisposed.Should().BeTrue();
    }

    [TestMethod]
    public async Task DisposingTheHostAsynchronouslyDisposesTheServiceProvider()
    {
      var host = CslaTestHost.Create(t => t.ConfigureServices(s => s.AddSingleton<DisposalSentinel>()));
      var sentinel = host.Services.GetRequiredService<DisposalSentinel>();

      await host.DisposeAsync();

      sentinel.IsDisposed.Should().BeTrue();
    }

    [TestMethod]
    public void TwoHostsDoNotShareAPrincipal()
    {
      // the AsyncLocal context manager keeps hosts isolated from one another
      using var first = CslaTestHost.Create(t => t.AsUser("alice", "Admin"));
      using var second = CslaTestHost.Create(t => t.AsUser("bob", "Users"));

      first.ApplicationContext.User.Identity.Name.Should().Be("alice");
      second.ApplicationContext.User.Identity.Name.Should().Be("bob");
      first.ApplicationContext.User.IsInRole("Users").Should().BeFalse();
    }

    [TestMethod]
    public async Task HostsUsedConcurrentlyKeepTheirOwnPrincipal()
    {
      async Task<string> FetchUserName(string name)
      {
        using var host = CslaTestHost.Create(t => t
          .AsUser(name)
          .ConfigureServices(s => s.AddScoped<INameSource, FakeNameSource>()));
        await Task.Yield();
        var root = await host.GetDataPortal<HostedRoot>().FetchAsync(1);
        return root.UserName;
      }

      var results = await Task.WhenAll(FetchUserName("alice"), FetchUserName("bob"), FetchUserName("carol"));

      results.Should().BeEquivalentTo(new[] { "alice", "bob", "carol" });
    }

    [TestMethod]
    public void ConfigureCslaThrowsForANullCallback()
    {
      var options = new CslaTestHostOptions();

      var act = () => options.ConfigureCsla(null);

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ConfigureServicesThrowsForANullCallback()
    {
      var options = new CslaTestHostOptions();

      var act = () => options.ConfigureServices(null);

      act.Should().Throw<ArgumentNullException>();
    }
  }
}
