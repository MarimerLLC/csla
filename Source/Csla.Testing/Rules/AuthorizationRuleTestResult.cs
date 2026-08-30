//-----------------------------------------------------------------------
// <copyright file="AuthorizationRuleTestResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Outcome of executing an authorization rule with AuthorizationRuleTester</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla.Testing.Rules
{
  /// <summary>
  /// The outcome of executing an authorization rule through
  /// <see cref="AuthorizationRuleTester"/>.
  /// </summary>
  /// <remarks>
  /// The result owns the services created for the rule, so
  /// <see cref="Context"/> stays usable after the rule has run. Disposing the
  /// result releases those services; the recorded outcome remains readable.
  /// </remarks>
  public sealed class AuthorizationRuleTestResult : IDisposable
  {
    private readonly IAuthorizationContext _context;
    private readonly IDisposable? _scope;

    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    /// <param name="context">The authorization context used to execute the rule.</param>
    /// <param name="scope">The services created for the rule, if any.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal AuthorizationRuleTestResult(IAuthorizationContext context, IDisposable? scope)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _scope = scope;
    }

    /// <summary>
    /// Releases the services created to run the rule.
    /// </summary>
    public void Dispose() => _scope?.Dispose();

    /// <summary>
    /// Gets the authorization context that was passed to the rule.
    /// </summary>
    public IAuthorizationContext Context => _context;

    /// <summary>
    /// Gets a value indicating whether the rule granted permission. A rule that
    /// sets no value denies permission.
    /// </summary>
    public bool HasPermission => _context.HasPermission;
  }
}
