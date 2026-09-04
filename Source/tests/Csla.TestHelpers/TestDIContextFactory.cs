//-----------------------------------------------------------------------
// <copyright file="TestDIContextFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory for DI context instances for use in tests</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using System.Security.Principal;
using Csla.Configuration;
using Csla.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.TestHelpers
{

  /// <summary>
  /// Factory for test DI contexts, for use in unit testing
  /// </summary>
  /// <remarks>
  /// Superseded by <see cref="CslaTestHost"/>, the supported public equivalent in the
  /// Csla.Testing package. This type is now a thin shim over it, so that the existing
  /// test suite keeps compiling while also exercising the shipped public API. New tests
  /// should use <see cref="CslaTestHost"/> directly; the remaining call sites here are
  /// migrated in a follow-up, after which this type is removed.
  /// </remarks>
  [Obsolete("Use Csla.Testing.CslaTestHost.Create instead. This shim is no longer used by the test suite and will be removed in a future release.")]
  public static class TestDIContextFactory
  {

    /// <summary>
    /// Create a test DI context for testing with a default authenticated user
    /// </summary>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateDefaultContext(Action<IServiceCollection> configureServices = null)
    {
      ClaimsPrincipal principal;

      // Create a default security principal
      principal = CreateDefaultClaimsPrincipal();

      // Delegate to the other overload to create the context
      return CreateContext(principal, configureServices);
    }

    /// <summary>
    /// Create a test DI context for testing with a specific authenticated user
    /// </summary>
    /// <param name="principal">The principal which is to be set as the security context for Csla operations</param>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateContext(ClaimsPrincipal principal, Action<IServiceCollection> configureServices = null)
    {
      return CreateContext(null, principal, configureServices);
    }

    /// <summary>
    /// Create a test DI context for testing with a specific authenticated user
    /// </summary>
    /// <param name="customCslaOptions">The options action that is used by the consumer to configure Csla</param>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateContext(Action<CslaOptions> customCslaOptions)
    {
      ClaimsPrincipal principal;

      principal = CreateDefaultClaimsPrincipal();
      return CreateContext(customCslaOptions, principal);
    }

    /// <summary>
    /// Create a test DI context for testing with a specific authenticated user
    /// </summary>
    /// <param name="customCslaOptions">The options action that is used by the consumer to configure Csla</param>
    /// <param name="principal">The principal which is to be set as the security context for Csla operations</param>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateContext(Action<CslaOptions> customCslaOptions, ClaimsPrincipal principal, Action<IServiceCollection> configureServices = null)
    {
      var host = CslaTestHost.Create(options =>
      {
        // These two registrations are what this suite has always run with, and are
        // deliberately not what the public API does by default. CslaTestHost applies
        // the caller's services last, so both win over the defaults.
        options.ConfigureServices(services =>
        {
          // several tests reach for ApplicationContextManagerUnitTests by casting
          // ApplicationContext.ContextManager, so keep registering it here
          services.AddSingleton<Core.IContextManager, ApplicationContextManagerUnitTests>();
          // the public AddCslaTesting leaves the dashboard at the framework default of
          // NullDashboard; this suite has always run with the real one
          services.TryAddSingleton<Server.Dashboard.IDashboard, Server.Dashboard.Dashboard>();
        });

        if (customCslaOptions is not null)
          options.ConfigureCsla(customCslaOptions);
        if (configureServices is not null)
          options.ConfigureServices(configureServices);

        if (principal is not null)
          options.AsPrincipal(principal);
      });

      return new TestDIContext(host.Services);
    }

    /// <summary>
    /// Create a default ClaimsPrincipal for use as the security context of Csla operations
    /// </summary>
    /// <returns>A default configured ClaimsPrincipal</returns>
    private static ClaimsPrincipal CreateDefaultClaimsPrincipal()
    {
      ClaimsIdentity identity;

      // Create a default security principal
      identity = new ClaimsIdentity(new GenericIdentity("Fred"));
      identity.AddClaim(new Claim(ClaimTypes.Role, "Users"));
      return new ClaimsPrincipal(identity);
    }

  }
}
