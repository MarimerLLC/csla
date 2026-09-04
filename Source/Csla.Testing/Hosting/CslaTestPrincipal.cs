//-----------------------------------------------------------------------
// <copyright file="CslaTestPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Carries the principal a test is configured to run as</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;

namespace Csla.Testing
{
  /// <summary>
  /// Carries the principal a test is configured to run as, so that the
  /// principal can be registered as a service rather than assigned to an
  /// <see cref="ApplicationContext"/> after the service provider is built.
  /// </summary>
  /// <remarks>
  /// This is what lets <see cref="Csla.Configuration.ServiceCollectionExtensions.AddCslaTesting(Microsoft.Extensions.DependencyInjection.IServiceCollection, Action{CslaTestingOptions})"/>
  /// support principals: an <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
  /// extension cannot reach into a provider that does not exist yet, so the
  /// principal is declared here and <see cref="CslaTestContextManager"/> seeds
  /// itself from it on first use.
  /// </remarks>
  public sealed class CslaTestPrincipal
  {
    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    /// <param name="principal">The principal the test runs as.</param>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    public CslaTestPrincipal(IPrincipal principal)
    {
      Principal = principal ?? throw new ArgumentNullException(nameof(principal));
    }

    /// <summary>
    /// Gets the principal the test runs as.
    /// </summary>
    public IPrincipal Principal { get; }
  }
}
