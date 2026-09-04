//-----------------------------------------------------------------------
// <copyright file="AddCslaTestingTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the AddCslaTesting service registration API</summary>
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
  /// Tests for <see cref="ServiceCollectionExtensions.AddCslaTesting(IServiceCollection, Action{CslaTestingOptions})"/>,
  /// the registration layer a test uses when it owns its own service collection.
  /// </summary>
  [TestClass]
  public class AddCslaTestingTests
  {
    [TestMethod]
    public void AddCslaTestingDoesNotAddCsla()
    {
      // it is purely additive: adding CSLA itself stays the caller's job
      var services = new ServiceCollection();

      services.AddCslaTesting();

      services.Should().NotContain(d => d.ServiceType == typeof(ApplicationContext));
    }

    [TestMethod]
    public void AddCslaTestingRegistersTheHostEnvironment()
    {
      using var provider = BuildProvider();

      var environment = provider.GetRequiredService<IHostEnvironment>();

      environment.Should().BeOfType<CslaTestHostEnvironment>();
    }

    [TestMethod]
    public void TestHostEnvironmentUsesAPortableContentRoot()
    {
      // the internal helper this replaces hardcoded C:\Windows\Temp
      var environment = new CslaTestHostEnvironment();

      environment.ContentRootPath.Should().Be(Path.GetTempPath());
    }

    [TestMethod]
    public void AddCslaTestingRegistersTheAsyncLocalContextManager()
    {
      using var provider = BuildProvider();

      var contextManager = provider.GetRequiredService<IContextManager>();

      contextManager.Should().BeOfType<CslaTestContextManager>();
      contextManager.Should().BeAssignableTo<ApplicationContextManagerAsyncLocal>(
        "tests must not be forced to run serially");
    }

    [TestMethod]
    public void TheDefaultUserIsAuthenticatedWithNoRoles()
    {
      using var provider = BuildProvider();

      var user = provider.GetRequiredService<ApplicationContext>().User;

      user.Identity!.IsAuthenticated.Should().BeTrue();
      user.Identity.Name.Should().Be("TestUser");
      ((ClaimsPrincipal)user).Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
    }

    [TestMethod]
    public void ThePrincipalIsSeededWithoutAnyPostBuildAssignment()
    {
      // the point of registering the principal as a service: an IServiceCollection
      // extension cannot reach into a provider that does not exist yet, so nothing
      // here assigns ApplicationContext.User after BuildServiceProvider
      using var provider = BuildProvider(t => t.AsUser("alice", "Admin"));

      var user = provider.GetRequiredService<ApplicationContext>().User;

      user.Identity!.Name.Should().Be("alice");
      user.IsInRole("Admin").Should().BeTrue();
    }

    [TestMethod]
    public void ThePrincipalIsSeededOnlyOnceNotPerAsyncFlow()
    {
      // CSLA deliberately leaves the user unset in places such as the server side of a
      // data portal call that does not flow the principal. If the manager re-seeded
      // every unseeded flow, the configured principal would reappear there and mask
      // exactly the state such a test needs to observe.
      using var provider = BuildProvider(t => t.AsUser("alice"));
      var contextManager = provider.GetRequiredService<IContextManager>();
      contextManager.GetUser().Identity!.Name.Should().Be("alice");

      contextManager.SetUser(new ClaimsPrincipal(new ClaimsIdentity()));

      contextManager.GetUser().Identity!.IsAuthenticated.Should().BeFalse();
    }

    [TestMethod]
    public void AsUserGrantsEveryRequestedRole()
    {
      using var provider = BuildProvider(t => t.AsUser("alice", "Admin", "Users"));

      var user = provider.GetRequiredService<ApplicationContext>().User;

      user.IsInRole("Admin").Should().BeTrue();
      user.IsInRole("Users").Should().BeTrue();
      user.IsInRole("Nope").Should().BeFalse();
    }

    [TestMethod]
    public void AsUnauthenticatedProducesAnAnonymousUser()
    {
      using var provider = BuildProvider(t => t.AsUnauthenticated());

      var user = provider.GetRequiredService<ApplicationContext>().User;

      user.Identity!.IsAuthenticated.Should().BeFalse();
    }

    [TestMethod]
    public void AsPrincipalUsesTheSuppliedPrincipal()
    {
      var identity = new ClaimsIdentity("custom", ClaimTypes.Name, ClaimTypes.Role);
      identity.AddClaim(new Claim(ClaimTypes.Name, "bob"));
      var principal = new ClaimsPrincipal(identity);

      using var provider = BuildProvider(t => t.AsPrincipal(principal));

      provider.GetRequiredService<ApplicationContext>().User.Should().BeSameAs(principal);
    }

    [TestMethod]
    public void ACallerSuppliedHostEnvironmentWins()
    {
      // every registration uses TryAdd, so the caller's own registration stands
      var services = new ServiceCollection();
      services.AddSingleton<IHostEnvironment>(new CslaTestHostEnvironment { ApplicationName = "Mine" });
      services.AddCsla();
      services.AddCslaTesting();

      using var provider = services.BuildServiceProvider();

      provider.GetRequiredService<IHostEnvironment>().ApplicationName.Should().Be("Mine");
    }

    [TestMethod]
    public void ACallerSuppliedContextManagerWins()
    {
      var services = new ServiceCollection();
      services.AddSingleton<IContextManager, CustomContextManager>();
      services.AddCsla();
      services.AddCslaTesting();

      using var provider = services.BuildServiceProvider();

      provider.GetRequiredService<IContextManager>().Should().BeOfType<CustomContextManager>();
    }

    [TestMethod]
    public void AddCslaTestingThrowsForANullServiceCollection()
    {
      var act = () => ((IServiceCollection)null).AddCslaTesting();

      act.Should().Throw<ArgumentNullException>();
    }

    private static ServiceProvider BuildProvider(Action<CslaTestingOptions> options = null)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddCslaTesting(options);
      return services.BuildServiceProvider();
    }
  }
}
