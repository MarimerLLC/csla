//-----------------------------------------------------------------------
// <copyright file="CslaTestContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context manager that seeds itself from the configured test principal</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;

namespace Csla.Testing
{
  /// <summary>
  /// Application context manager for unit tests. Stores its state in
  /// <see cref="System.Threading.AsyncLocal{T}"/> fields, and seeds the current
  /// user from the <see cref="CslaTestPrincipal"/> registered for the test.
  /// </summary>
  /// <remarks>
  /// <para>
  /// State is held per async flow rather than in static fields, so tests using
  /// this manager do not have to be run serially.
  /// </para>
  /// <para>
  /// Seeding is what allows the principal to be configured at registration
  /// time. The principal is not assigned to an <see cref="ApplicationContext"/>
  /// after the container is built; instead the first code to ask for the
  /// current user gets the configured principal.
  /// </para>
  /// </remarks>
  public class CslaTestContextManager : Core.ApplicationContextManagerAsyncLocal
  {
    private readonly CslaTestPrincipal _testPrincipal;
    private bool _seeded;

    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    /// <param name="testPrincipal">The principal the test is configured to run as.</param>
    /// <exception cref="ArgumentNullException"><paramref name="testPrincipal"/> is <see langword="null"/>.</exception>
    public CslaTestContextManager(CslaTestPrincipal testPrincipal)
    {
      _testPrincipal = testPrincipal ?? throw new ArgumentNullException(nameof(testPrincipal));
    }

    /// <summary>
    /// Gets the current user principal, seeding it from the configured test
    /// principal the first time a user is requested.
    /// </summary>
    public override IPrincipal GetUser()
    {
      if (!_seeded)
        SetUser(_testPrincipal.Principal);
      return base.GetUser();
    }

    /// <inheritdoc />
    public override void SetUser(IPrincipal principal)
    {
      base.SetUser(principal);
      // set only after the base call, so a null principal throws without
      // marking the manager as seeded
      _seeded = true;
    }
  }
}
