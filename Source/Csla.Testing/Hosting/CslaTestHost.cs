//-----------------------------------------------------------------------
// <copyright file="CslaTestHost.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Self-contained CSLA .NET container for use in a unit test</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Testing
{
  /// <summary>
  /// A self-contained, configured CSLA .NET container for a unit test. Owns the
  /// service provider it builds, and exposes the <see cref="ApplicationContext"/>
  /// and data portals resolved from it.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is the convenient way to stand up CSLA .NET in a test. A test that
  /// needs to own its own <see cref="IServiceCollection"/> can use
  /// <c>AddCslaTesting</c> directly instead; this type is built on top of it.
  /// </para>
  /// <para>
  /// The host is disposable, and disposing it disposes the service provider it
  /// owns. It carries no dependency on any test framework, so it can be created
  /// from a constructor, a class or test initialize method, or an
  /// <c>IAsyncLifetime</c>.
  /// </para>
  /// <example>
  /// <code>
  /// using var host = CslaTestHost.Create(t => t
  ///   .ConfigureCsla(o => o.DataPortal(/* ... */))
  ///   .AsUser("alice", "Admin")
  ///   .ConfigureServices(s => s.AddSingleton&lt;IOrderRepository, FakeOrderRepository&gt;()));
  ///
  /// var portal = host.GetDataPortal&lt;Order&gt;();
  /// var order = await portal.FetchAsync(42);
  /// </code>
  /// </example>
  /// </remarks>
  public sealed class CslaTestHost : IDisposable, IAsyncDisposable
  {
    private readonly ServiceProvider _provider;

    private CslaTestHost(ServiceProvider provider, ApplicationContext applicationContext)
    {
      _provider = provider;
      ApplicationContext = applicationContext;
    }

    /// <summary>
    /// Creates a test host using the default configuration: a local data portal
    /// and an authenticated user named <c>TestUser</c> holding no roles.
    /// </summary>
    public static CslaTestHost Create()
    {
      return Create(null);
    }

    /// <summary>
    /// Creates a test host.
    /// </summary>
    /// <param name="options">Callback used to configure the host.</param>
    /// <remarks>
    /// Services registered through
    /// <see cref="CslaTestHostOptions.ConfigureServices"/> are added last, so
    /// that they win over the registrations CSLA .NET and the testing services
    /// would otherwise make.
    /// </remarks>
    public static CslaTestHost Create(Action<CslaTestHostOptions>? options)
    {
      var hostOptions = new CslaTestHostOptions();
      options?.Invoke(hostOptions);

      var services = new ServiceCollection();
      services.AddCsla(hostOptions.ApplyCsla);
      services.AddCslaTesting(hostOptions.ApplyTesting);
      // the caller's registrations go last, so they win: a service resolves to the
      // last registration for its type, and any TryAdd made here is skipped outright
      // if the caller registered that service themselves
      hostOptions.ApplyServices(services);

      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      // CslaTestContextManager seeds itself from the registered principal, but a
      // caller is free to supply a context manager of their own; assigning the
      // principal here means the configured user takes effect either way
      applicationContext.User = hostOptions.Principal;

      return new CslaTestHost(provider, applicationContext);
    }

    /// <summary>
    /// Gets the service provider owned by this host.
    /// </summary>
    public IServiceProvider Services => _provider;

    /// <summary>
    /// Gets the application context for this host.
    /// </summary>
    public ApplicationContext ApplicationContext { get; }

    /// <summary>
    /// Gets a data portal for the specified business type.
    /// </summary>
    /// <typeparam name="T">The type the data portal serves.</typeparam>
    public IDataPortal<T> GetDataPortal<T>() where T : ICslaObject
    {
      return _provider.GetRequiredService<IDataPortal<T>>();
    }

    /// <summary>
    /// Gets a child data portal for the specified business type.
    /// </summary>
    /// <typeparam name="T">The type the child data portal serves.</typeparam>
    public IChildDataPortal<T> GetChildDataPortal<T>() where T : ICslaObject
    {
      return _provider.GetRequiredService<IChildDataPortal<T>>();
    }

    /// <summary>
    /// Creates a service scope, for a test that needs to exercise behavior that
    /// varies by scope.
    /// </summary>
    public IServiceScope CreateScope() => _provider.CreateScope();

    /// <summary>
    /// Disposes the service provider owned by this host.
    /// </summary>
    public void Dispose() => _provider.Dispose();

    /// <summary>
    /// Asynchronously disposes the service provider owned by this host.
    /// </summary>
    public ValueTask DisposeAsync() => _provider.DisposeAsync();
  }
}
