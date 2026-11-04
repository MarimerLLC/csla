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
      new SessionPurgingSettings(Interval, Expiration),
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
    _logger.Entries.Should().ContainSingle(e => e.Level == LogLevel.Warning && e.Exception is InvalidOperationException);
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
  public void AddServerSideBlazor_WithStateManagement_RegistersPurgingWithDefaults()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddServerSideBlazor(b => b.UseInMemoryApplicationContextManager = false);

    services.Should().ContainSingle(d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(SessionPurgingService));
    GetSettings(services).Should().Be(new SessionPurgingSettings(TimeSpan.FromMinutes(5), TimeSpan.FromHours(1)));
    services.Should().Contain(d => d.ServiceType == typeof(TimeProvider));
  }

  [TestMethod]
  public void AddServerSideBlazor_AppliesConfiguredPurgeSchedule()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddServerSideBlazor(b =>
    {
      b.UseInMemoryApplicationContextManager = false;
      b.SessionPurgeInterval = Interval;
      b.SessionExpiration = Expiration;
    });

    GetSettings(services).Should().Be(new SessionPurgingSettings(Interval, Expiration));
  }

  [TestMethod]
  public void AddServerSideBlazor_PurgeExpiredSessionsFalse_DoesNotRegisterPurging()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddServerSideBlazor(b =>
    {
      b.UseInMemoryApplicationContextManager = false;
      b.PurgeExpiredSessions = false;
      b.SessionPurgeInterval = TimeSpan.Zero;
    });

    services.Should().NotContain(d => d.ServiceType == typeof(IHostedService));
    services.Should().NotContain(d => d.ServiceType == typeof(SessionPurgingSettings));
  }

  [TestMethod]
  public void AddServerSideBlazor_InMemoryApplicationContextManager_DoesNotRegisterPurging()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddServerSideBlazor();

    services.Should().NotContain(d => d.ServiceType == typeof(IHostedService));
  }

  [TestMethod]
  public void AddServerSideBlazor_ResolvesPurgingService()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor(b => b.UseInMemoryApplicationContextManager = false));

    using var provider = services.BuildServiceProvider();

    provider.GetServices<IHostedService>().Should().ContainSingle(s => s is SessionPurgingService);
  }

  [TestMethod]
  [DataRow(1L)]
  [DataRow(4_294_967_294L)]
  public void AddServerSideBlazor_IntervalAtTimerLimits_IsAccepted(long milliseconds)
  {
    var services = new ServiceCollection();

    new CslaOptions(services).AddServerSideBlazor(b =>
    {
      b.UseInMemoryApplicationContextManager = false;
      b.SessionPurgeInterval = TimeSpan.FromMilliseconds(milliseconds);
    });

    // The timer accepts the same values, so the service can start.
    using var timer = new PeriodicTimer(GetSettings(services).Interval);
  }

  [TestMethod]
  [DataRow(0L)]
  [DataRow(-10_000L)]
  [DataRow(9_999L)]
  [DataRow(42_949_672_950_000L)]
  [DataRow(500L * TimeSpan.TicksPerDay)]
  public void AddServerSideBlazor_IntervalOutsideTimerRange_Throws(long ticks)
  {
    var services = new ServiceCollection();

    var act = () => new CslaOptions(services).AddServerSideBlazor(b =>
    {
      b.UseInMemoryApplicationContextManager = false;
      b.SessionPurgeInterval = TimeSpan.FromTicks(ticks);
    });

    act.Should().Throw<ArgumentOutOfRangeException>();
  }

  [TestMethod]
  [DataRow(0)]
  [DataRow(-1)]
  public void AddServerSideBlazor_NonPositiveExpiration_Throws(int seconds)
  {
    var services = new ServiceCollection();

    var act = () => new CslaOptions(services).AddServerSideBlazor(b =>
    {
      b.UseInMemoryApplicationContextManager = false;
      b.SessionExpiration = TimeSpan.FromSeconds(seconds);
    });

    act.Should().Throw<ArgumentOutOfRangeException>();
  }

  private static SessionPurgingSettings GetSettings(ServiceCollection services) =>
    (SessionPurgingSettings)services.Single(d => d.ServiceType == typeof(SessionPurgingSettings)).ImplementationInstance;

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
