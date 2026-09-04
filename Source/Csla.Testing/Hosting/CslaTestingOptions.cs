//-----------------------------------------------------------------------
// <copyright file="CslaTestingOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options controlling the CSLA .NET services added for a unit test</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;

namespace Csla.Testing
{
  /// <summary>
  /// Options controlling the CSLA .NET testing services added to a service
  /// collection by <c>AddCslaTesting</c>.
  /// </summary>
  /// <example>
  /// <code>
  /// services.AddCslaTesting(t => t.AsUser("alice", "Admin"));
  /// </code>
  /// </example>
  public class CslaTestingOptions
  {
    /// <summary>
    /// Gets the principal the test runs as. Defaults to an authenticated user
    /// named <c>TestUser</c> holding no roles.
    /// </summary>
    public IPrincipal Principal { get; private set; } = TestPrincipalFactory.CreateDefault();

    /// <summary>
    /// Runs the test as an authenticated user with the specified name and roles.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="roles">Roles held by the user.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="roles"/> is <see langword="null"/>.</exception>
    public CslaTestingOptions AsUser(string name, params string[] roles)
    {
      Principal = TestPrincipalFactory.CreateUser(name, roles);
      return this;
    }

    /// <summary>
    /// Runs the test as an anonymous, unauthenticated user.
    /// </summary>
    /// <returns>This instance, to support method chaining.</returns>
    public CslaTestingOptions AsUnauthenticated()
    {
      Principal = TestPrincipalFactory.CreateUnauthenticated();
      return this;
    }

    /// <summary>
    /// Runs the test as the specified principal.
    /// </summary>
    /// <param name="principal">The principal to use.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    public CslaTestingOptions AsPrincipal(IPrincipal principal)
    {
      Principal = principal ?? throw new ArgumentNullException(nameof(principal));
      return this;
    }
  }
}
