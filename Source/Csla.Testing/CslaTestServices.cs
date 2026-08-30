//-----------------------------------------------------------------------
// <copyright file="CslaTestServices.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Minimal CSLA .NET service bootstrap used by the rule testers</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Csla.Testing
{
  /// <summary>
  /// Creates the minimal set of CSLA .NET services required to execute a
  /// business or authorization rule in isolation.
  /// </summary>
  /// <remarks>
  /// This type is deliberately internal. A supported, public test host is
  /// tracked separately; when it exists the rule testers switch to it without
  /// any change to their own public API.
  /// </remarks>
  internal static class CslaTestServices
  {
    /// <summary>
    /// Builds a service provider containing the CSLA .NET services and returns
    /// a disposable holder exposing the resulting <see cref="ApplicationContext"/>.
    /// </summary>
    /// <param name="configureCsla">Optional CSLA .NET configuration callback.</param>
    /// <param name="configureServices">Optional service registration callback.</param>
    public static CslaTestScope CreateScope(Action<CslaOptions>? configureCsla, Action<IServiceCollection>? configureServices)
    {
      var services = new ServiceCollection();
      services.TryAddTransient<IHostEnvironment, CslaTestHostEnvironment>();
      services.AddLogging();
      services.AddCsla(configureCsla);
      configureServices?.Invoke(services);

      var provider = services.BuildServiceProvider();
      return new CslaTestScope(provider);
    }
  }

  /// <summary>
  /// Owns the service provider created for a single rule execution and
  /// exposes the <see cref="Csla.ApplicationContext"/> resolved from it.
  /// </summary>
  internal sealed class CslaTestScope : IDisposable
  {
    private readonly ServiceProvider _provider;

    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    /// <param name="provider">Service provider owned by this scope.</param>
    public CslaTestScope(ServiceProvider provider)
    {
      _provider = provider;
      ApplicationContext = provider.GetRequiredService<ApplicationContext>();
    }

    /// <summary>
    /// Gets the application context for this scope.
    /// </summary>
    public ApplicationContext ApplicationContext { get; }

    /// <summary>
    /// Disposes the service provider owned by this scope.
    /// </summary>
    public void Dispose() => _provider.Dispose();
  }

  /// <summary>
  /// Minimal <see cref="IHostEnvironment"/> implementation so types that take a
  /// dependency on the hosting environment can be resolved in a unit test.
  /// </summary>
  internal sealed class CslaTestHostEnvironment : IHostEnvironment
  {
    /// <inheritdoc />
    public string EnvironmentName { get; set; } = "Production";

    /// <inheritdoc />
    public string ApplicationName { get; set; } = "Csla.Testing";

    /// <inheritdoc />
    public string ContentRootPath { get; set; } = Path.GetTempPath();

    /// <inheritdoc />
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
  }
}
