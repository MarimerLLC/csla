//-----------------------------------------------------------------------
// <copyright file="SessionPurgingService.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Background service that purges expired sessions</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.State;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Csla.Blazor.State;

/// <summary>
/// Background service that periodically calls
/// <see cref="ISessionManager.PurgeSessions"/>.
/// </summary>
/// <remarks>
/// A failed purge is logged and the next one runs on schedule. If the
/// session manager doesn't support purging, such as when the in-memory
/// application context manager is used, the service logs a warning and stops.
/// </remarks>
internal sealed partial class SessionPurgingService(
  ISessionManager sessionManager,
  SessionPurgingOptions options,
  TimeProvider timeProvider,
  ILogger<SessionPurgingService> logger) : BackgroundService
{
  /// <inheritdoc />
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var timer = new PeriodicTimer(options.Interval, timeProvider);
    try
    {
      while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
      {
        try
        {
          sessionManager.PurgeSessions(options.Expiration);
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

  [LoggerMessage(Level = LogLevel.Warning, Message = "Session purging stopped, because the session manager {SessionManager} does not support purging. Set UseInMemoryApplicationContextManager to false to use Blazor state management.")]
  private partial void LogPurgeNotSupported(Exception exception, string? sessionManager);

  [LoggerMessage(Level = LogLevel.Error, Message = "Failed to purge expired sessions; the next purge runs on schedule")]
  private partial void LogPurgeFailed(Exception exception);
}
