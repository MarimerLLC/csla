//-----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Adds the CSLA .NET services needed to run a unit test</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension methods to add the supporting services a CSLA .NET unit test
  /// needs to a service collection.
  /// </summary>
  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Adds the supporting services a CSLA .NET unit test needs, running the
    /// test as the default principal.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddCslaTesting(this IServiceCollection services)
    {
      return AddCslaTesting(services, null);
    }

    /// <summary>
    /// Adds the supporting services a CSLA .NET unit test needs.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="options">Callback used to configure the testing services.</param>
    /// <returns>The service collection, to support method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method is purely additive: it does <b>not</b> call <c>AddCsla</c>.
    /// Adding the CSLA .NET services themselves stays the caller's job, so that
    /// they keep full control of how CSLA .NET is configured:
    /// </para>
    /// <example>
    /// <code>
    /// var services = new ServiceCollection();
    /// services.AddCsla(o => o.DataPortal(/* ... */));
    /// services.AddCslaTesting(t => t.AsUser("alice", "Admin"));
    /// services.AddSingleton&lt;IOrderRepository, FakeOrderRepository&gt;();
    /// </code>
    /// </example>
    /// <para>
    /// Use <see cref="CslaTestHost"/> instead when a test does not need to own
    /// the service collection; it calls both methods.
    /// </para>
    /// <para>
    /// Every registration made here uses <c>TryAdd</c> semantics, so a
    /// registration the caller has already made wins. In particular, supplying
    /// an <see cref="Microsoft.Extensions.Hosting.IHostEnvironment"/> or a
    /// <see cref="Csla.Core.IContextManager"/> of your own before calling this
    /// method leaves it in place.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddCslaTesting(this IServiceCollection services, Action<CslaTestingOptions>? options)
    {
      if (services is null)
        throw new ArgumentNullException(nameof(services));

      var testingOptions = new CslaTestingOptions();
      options?.Invoke(testingOptions);

      services.TryAddTransient<IHostEnvironment, CslaTestHostEnvironment>();
      services.AddLogging();

      // the principal is registered rather than assigned after the provider is
      // built, so that it is available to a test that owns its own container;
      // CslaTestContextManager seeds itself from it on first use
      services.TryAddSingleton(new CslaTestPrincipal(testingOptions.Principal));
      services.TryAddSingleton<Core.IContextManager, CslaTestContextManager>();

      return services;
    }
  }
}
