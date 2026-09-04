//-----------------------------------------------------------------------
// <copyright file="CslaTestHostOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options used to build a CslaTestHost</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Testing
{
  /// <summary>
  /// Options used to build a <see cref="CslaTestHost"/>.
  /// </summary>
  /// <example>
  /// <code>
  /// using var host = CslaTestHost.Create(t => t
  ///   .ConfigureCsla(o => o.DataPortal(/* ... */))
  ///   .AsUser("alice", "Admin")
  ///   .ConfigureServices(s => s.AddSingleton&lt;IOrderRepository, FakeOrderRepository&gt;()));
  /// </code>
  /// </example>
  public class CslaTestHostOptions
  {
    private readonly CslaTestingOptions _testingOptions = new();
    private Action<CslaOptions>? _configureCsla;
    private Action<IServiceCollection>? _configureServices;

    /// <summary>
    /// Gets the principal the test runs as. Defaults to an authenticated user
    /// named <c>TestUser</c> holding no roles.
    /// </summary>
    public IPrincipal Principal => _testingOptions.Principal;

    /// <summary>
    /// Configures CSLA .NET for the test. Calling this more than once composes
    /// the callbacks rather than replacing the previous one.
    /// </summary>
    /// <param name="configure">Callback used to configure CSLA .NET.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <see langword="null"/>.</exception>
    public CslaTestHostOptions ConfigureCsla(Action<CslaOptions> configure)
    {
      if (configure is null)
        throw new ArgumentNullException(nameof(configure));

      _configureCsla += configure;
      return this;
    }

    /// <summary>
    /// Registers services for the test, such as fakes for the application's own
    /// services. Calling this more than once composes the callbacks rather than
    /// replacing the previous one.
    /// </summary>
    /// <param name="configure">Callback used to register services.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <remarks>
    /// These registrations are applied after CSLA .NET's own, so a service
    /// registered here wins over the one the test host would otherwise add.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <see langword="null"/>.</exception>
    public CslaTestHostOptions ConfigureServices(Action<IServiceCollection> configure)
    {
      if (configure is null)
        throw new ArgumentNullException(nameof(configure));

      _configureServices += configure;
      return this;
    }

    /// <summary>
    /// Runs the test as an authenticated user with the specified name and roles.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="roles">Roles held by the user.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="roles"/> is <see langword="null"/>.</exception>
    public CslaTestHostOptions AsUser(string name, params string[] roles)
    {
      _testingOptions.AsUser(name, roles);
      return this;
    }

    /// <summary>
    /// Runs the test as an anonymous, unauthenticated user.
    /// </summary>
    /// <returns>This instance, to support method chaining.</returns>
    public CslaTestHostOptions AsUnauthenticated()
    {
      _testingOptions.AsUnauthenticated();
      return this;
    }

    /// <summary>
    /// Runs the test as the specified principal.
    /// </summary>
    /// <param name="principal">The principal to use.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    public CslaTestHostOptions AsPrincipal(IPrincipal principal)
    {
      _testingOptions.AsPrincipal(principal);
      return this;
    }

    /// <summary>
    /// Applies the registrations the caller supplied through
    /// <see cref="ConfigureServices"/>.
    /// </summary>
    internal void ApplyServices(IServiceCollection services) => _configureServices?.Invoke(services);

    /// <summary>
    /// Applies the CSLA .NET configuration the caller supplied through
    /// <see cref="ConfigureCsla"/>.
    /// </summary>
    internal void ApplyCsla(CslaOptions options) => _configureCsla?.Invoke(options);

    /// <summary>
    /// Applies the testing configuration, so that the host and
    /// <c>AddCslaTesting</c> agree about the principal.
    /// </summary>
    internal void ApplyTesting(CslaTestingOptions options) => options.AsPrincipal(_testingOptions.Principal);
  }
}
