//-----------------------------------------------------------------------
// <copyright file="SessionPurgingService.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Background service that purges expired sessions</summary>
//-----------------------------------------------------------------------

using Csla.State;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Csla.Blazor.State;

/// <summary>
/// The purge schedule for <see cref="SessionPurgingService"/>.
/// </summary>
/// <param name="Interval">How often sessions are purged.</param>
/// <param name="Expiration">How long a session can go untouched before it is purged.</param>
internal sealed record SessionPurgingSettings(TimeSpan Interval, TimeSpan Expiration);

/// <summary>
/// Background service that periodically calls
/// <see cref="ISessionManager.PurgeSessions"/>.
/// </summary>
/// <remarks>
/// A failed purge is logged and the next one runs on schedule. If the
/// session manager doesn't support purging, the service logs a warning and stops.
/// </remarks>
internal sealed partial class SessionPurgingService(
  ISessionManager sessionManager,
  SessionPurgingSettings settings,
  TimeProvider timeProvider,
  ILogger<SessionPurgingService> logger) : BackgroundService
{
  /// <inheritdoc />
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var timer = new PeriodicTimer(settings.Interval, timeProvider);
    try
    {
      while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
      {
        try
        {
          sessionManager.PurgeSessions(settings.Expiration);
        }
        catch (Exception ex) when (ex is NotSupportedException or NotImplementedException)
        {
          LogPurgeNotSupported(ex, sessionManager.GetType().FullName);
          return;
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
          LogPurgeFailed(ex);
        }
      }
    }
    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
    {
      // The host is stopping.
    }
  }

  [LoggerMessage(Level = LogLevel.Warning, Message = "Session purging stopped, because the session manager {SessionManager} does not support purging")]
  private partial void LogPurgeNotSupported(Exception exception, string? sessionManager);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to purge expired sessions; the next purge runs on schedule")]
  private partial void LogPurgeFailed(Exception exception);
}
