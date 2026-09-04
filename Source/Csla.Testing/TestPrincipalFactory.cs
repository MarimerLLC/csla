//-----------------------------------------------------------------------
// <copyright file="TestPrincipalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates principal objects for use when testing rules</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using System.Security.Principal;

namespace Csla.Testing
{
  /// <summary>
  /// Creates the principal objects used by the rule testers.
  /// </summary>
  internal static class TestPrincipalFactory
  {
    /// <summary>
    /// The authentication type given to an authenticated test identity.
    /// </summary>
    public const string AuthenticationType = "CslaTesting";

    /// <summary>
    /// The name given to the default authenticated test identity.
    /// </summary>
    public const string DefaultUserName = "TestUser";

    /// <summary>
    /// Creates an authenticated principal with the specified name and roles.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="roles">Roles held by the user.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="roles"/> is <see langword="null"/>.</exception>
    public static IPrincipal CreateUser(string name, params string[] roles)
    {
      if (name is null)
        throw new ArgumentNullException(nameof(name));
      if (roles is null)
        throw new ArgumentNullException(nameof(roles));

      var identity = new ClaimsIdentity(AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
      identity.AddClaim(new Claim(ClaimTypes.Name, name));
      foreach (var role in roles)
      {
        if (role is not null)
          identity.AddClaim(new Claim(ClaimTypes.Role, role));
      }
      return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Creates the default principal for a test: an authenticated user holding
    /// no roles.
    /// </summary>
    /// <remarks>
    /// Authenticated, so that a first use of the test helpers is not met with a
    /// confusing authorization failure; without roles, so that a test which
    /// depends on a role has to say so.
    /// </remarks>
    public static IPrincipal CreateDefault() => CreateUser(DefaultUserName);

    /// <summary>
    /// Creates an unauthenticated principal.
    /// </summary>
    public static IPrincipal CreateUnauthenticated() => new ClaimsPrincipal(new ClaimsIdentity());
  }
}
