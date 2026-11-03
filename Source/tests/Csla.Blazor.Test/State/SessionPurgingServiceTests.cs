//-----------------------------------------------------------------------
// <copyright file="SessionPurgingServiceTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the session purging background service</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using FluentAssertions;
using Csla.Blazor.State;
using Csla.Configuration;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.Test.State;

[TestClass]
public class SessionPurgingServiceTests
{
  private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
  private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(30);
  private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);
  private static readonly TimeSpan NoCallTimeout = TimeSpan.FromMilliseconds(200);

  private SignalingTimeProvider _timeProvider;
  private FakeSessionManager _sessionManager;
  private ListLogger _logger;
  private SessionPurgingService _service;

  [TestInitialize]
  public void Initialize()
  {
    _timeProvider = new SignalingTimeProvider();
    _sessionManager = new FakeSessionManager();
    _logger = new ListLogger();
    _service = new SessionPurgingService(
      _sessionManager,
      new SessionPurgingOptions { Interval = Interval, Expiration = Expiration },
      _timeProvider,
      _logger);
  }

  [TestCleanup]
  public async Task Cleanup()
  {
    await _service.StopAsync(CancellationToken.None);
    _service.Dispose();
  }

  [TestMethod]
  public async Task Purges_OncePerInterval_WithConfiguredExpiration()
  {
    await StartServiceAsync();

    _timeProvider.Advance(Interval - TimeSpan.FromSeconds(1));
    (await _sessionManager.Purged.WaitAsync(NoCallTimeout)).Should().BeFalse();

    _timeProvider.Advance(TimeSpan.FromSeconds(1));
    (await _sessionManager.Purged.WaitAsync(WaitTimeout)).Should().BeTrue();

    _timeProvider.Advance(Interval);
    (await _sessionManager.Purged.WaitAsync(WaitTimeout)).Should().BeTrue();

    _sessionManager.Expirations.Should().Equal(Expiration, Expiration);
  }

  [TestMethod]
  public async Task FailedPurge_IsLogged_AndNextIntervalPurgesAgain()
  {
    _sessionManager.ExceptionToThrow = new InvalidOperationException("store unavailable");
    await StartServiceAsync();

    _timeProvider.Advance(Interval);
    (await _sessionManager.Purged.WaitAsync(WaitTimeout)).Should().BeTrue();

    _sessionManager.ExceptionToThrow = null;
    _timeProvider.Advance(Interval);
    (await _sessionManager.Purged.WaitAsync(WaitTimeout)).Should().BeTrue();

    _sessionManager.Expirations.Should().HaveCount(2);
    _service.ExecuteTask.IsCompleted.Should().BeFalse();
    _logger.Entries.Should().ContainSingle(e => e.Level == LogLevel.Error && e.Exception is InvalidOperationException);
  }

  [TestMethod]
  [DataRow(typeof(NotSupportedException))]
  [DataRow(typeof(NotImplementedException))]
  public async Task UnsupportedPurge_LogsWarning_AndStopsService(Type exceptionType)
  {
    _sessionManager.ExceptionToThrow = (Exception)Activator.CreateInstance(exceptionType);
    await StartServiceAsync();

    _timeProvider.Advance(Interval);
    (await _sessionManager.Purged.WaitAsync(WaitTimeout)).Should().BeTrue();
    await _service.ExecuteTask.WaitAsync(WaitTimeout);

    _timeProvider.Advance(Interval);
    (await _sessionManager.Purged.WaitAsync(NoCallTimeout)).Should().BeFalse();

    _service.ExecuteTask.IsCompletedSuccessfully.Should().BeTrue();
    _logger.Entries.Should().ContainSingle(e => e.Level == LogLevel.Warning && e.Exception.GetType() == exceptionType);
  }

  [TestMethod]
  public async Task StopAsync_EndsWaitingLoop()
  {
    await StartServiceAsync();

    await _service.StopAsync(CancellationToken.None).WaitAsync(WaitTimeout);

    _service.ExecuteTask.IsCompletedSuccessfully.Should().BeTrue();
    _sessionManager.Expirations.Should().BeEmpty();
  }

  [TestMethod]
  public void AddSessionPurging_RegistersHostedServiceAndDefaultOptions()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddSessionPurging();

    services.Should().ContainSingle(d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(SessionPurgingService));
    var options = (SessionPurgingOptions)services.Single(d => d.ServiceType == typeof(SessionPurgingOptions)).ImplementationInstance;
    options.Interval.Should().Be(TimeSpan.FromMinutes(5));
    options.Expiration.Should().Be(TimeSpan.FromHours(1));
    services.Should().Contain(d => d.ServiceType == typeof(TimeProvider));
  }

  [TestMethod]
  public void AddSessionPurging_AppliesConfiguredOptions()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddSessionPurging(o =>
    {
      o.Interval = Interval;
      o.Expiration = Expiration;
    });

    var options = (SessionPurgingOptions)services.Single(d => d.ServiceType == typeof(SessionPurgingOptions)).ImplementationInstance;
    options.Interval.Should().Be(Interval);
    options.Expiration.Should().Be(Expiration);
  }

  [TestMethod]
  public void AddSessionPurging_ResolvesServiceWithServerSideBlazor()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor(b => b.UseInMemoryApplicationContextManager = false)
      .AddSessionPurging());

    using var provider = services.BuildServiceProvider();

    provider.GetServices<IHostedService>().Should().ContainSingle(s => s is SessionPurgingService);
  }

  [TestMethod]
  [DataRow(0, 60)]
  [DataRow(-1, 60)]
  [DataRow(60, 0)]
  [DataRow(60, -1)]
  public void AddSessionPurging_NonPositiveTimes_Throw(int intervalSeconds, int expirationSeconds)
  {
    var services = new ServiceCollection();

    var act = () => new CslaOptions(services).AddSessionPurging(o =>
    {
      o.Interval = TimeSpan.FromSeconds(intervalSeconds);
      o.Expiration = TimeSpan.FromSeconds(expirationSeconds);
    });

    act.Should().Throw<ArgumentOutOfRangeException>();
  }

  // ExecuteAsync runs on a background thread, so wait for the timer
  // before advancing the clock.
  private async Task StartServiceAsync()
  {
    await _service.StartAsync(CancellationToken.None);
    (await _timeProvider.TimerCreated.WaitAsync(WaitTimeout)).Should().BeTrue();
  }

  private sealed class SignalingTimeProvider : FakeTimeProvider
  {
    public SemaphoreSlim TimerCreated { get; } = new(0);

    public override ITimer CreateTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
    {
      var timer = base.CreateTimer(callback, state, dueTime, period);
      TimerCreated.Release();
      return timer;
    }
  }

  private sealed class FakeSessionManager : ISessionManager
  {
    public SemaphoreSlim Purged { get; } = new(0);
    public ConcurrentQueue<TimeSpan> Expirations { get; } = new();
    public Exception ExceptionToThrow { get; set; }

    public void PurgeSessions(TimeSpan expiration)
    {
      Expirations.Enqueue(expiration);
      try
      {
        if (ExceptionToThrow is not null)
          throw ExceptionToThrow;
      }
      finally
      {
        Purged.Release();
      }
    }

    public Session GetSession() => throw new NotImplementedException();
    public void UpdateSession(Session newSession) => throw new NotImplementedException();
    public Task<Session> RetrieveSession(TimeSpan timeout) => throw new NotImplementedException();
    public Task<Session> RetrieveSession(CancellationToken ct) => throw new NotImplementedException();
    public Session GetCachedSession() => throw new NotImplementedException();
    public Task SendSession(TimeSpan timeout) => throw new NotImplementedException();
    public Task SendSession(CancellationToken ct) => throw new NotImplementedException();
  }

  private sealed class ListLogger : ILogger<SessionPurgingService>
  {
    public ConcurrentQueue<(LogLevel Level, Exception Exception)> Entries { get; } = new();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      => Entries.Enqueue((logLevel, exception));
  }
}
