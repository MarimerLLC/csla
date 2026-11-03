//-----------------------------------------------------------------------
// <copyright file="SessionPurgingConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Configuration extensions for the session purging background service</summary>
//-----------------------------------------------------------------------

using Csla.Blazor.State;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration;

/// <summary>
/// Implement extension methods to register the background
/// service that purges expired Blazor sessions.
/// </summary>
public static class SessionPurgingConfigurationExtensions
{
  /// <summary>
  /// Registers a hosted background service that periodically calls
  /// <see cref="ISessionManager.PurgeSessions"/> to remove expired
  /// Blazor sessions from the <see cref="ISessionStore"/>.
  /// </summary>
  /// <param name="config">The <see cref="CslaOptions"/> instance.</param>
  /// <param name="options">An optional delegate to configure the purge interval and session expiration.</param>
  /// <returns>The <see cref="CslaOptions"/> instance.</returns>
  /// <remarks>
  /// Sessions are only stored when <c>AddServerSideBlazor</c> is called with
  /// <c>UseInMemoryApplicationContextManager</c> set to <see langword="false"/>.
  /// Otherwise the service logs a warning and stops.
  /// <para>
  /// A failed purge is logged, and the next purge runs on schedule.
  /// To read the settings from configuration, bind them in the delegate, for example
  /// <c>o => builder.Configuration.GetSection("Csla:SessionPurging").Bind(o)</c>.
  /// </para>
  /// </remarks>
  /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentOutOfRangeException">The interval or expiration is not greater than zero.</exception>
  public static CslaOptions AddSessionPurging(this CslaOptions config, Action<SessionPurgingOptions>? options = null)
  {
    ArgumentNullException.ThrowIfNull(config);

    var purgingOptions = new SessionPurgingOptions();
    options?.Invoke(purgingOptions);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(purgingOptions.Interval, TimeSpan.Zero, $"{nameof(SessionPurgingOptions)}.{nameof(SessionPurgingOptions.Interval)}");
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(purgingOptions.Expiration, TimeSpan.Zero, $"{nameof(SessionPurgingOptions)}.{nameof(SessionPurgingOptions.Expiration)}");

    config.Services.AddSingleton(purgingOptions);
    config.Services.TryAddSingleton(TimeProvider.System);
    config.Services.AddHostedService<SessionPurgingService>();
    return config;
  }
}
