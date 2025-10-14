//-----------------------------------------------------------------------
// <copyright file="OpenTelemetryDashboard.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>OpenTelemetry dashboard implementation for data portal</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;

namespace Csla.Server.Dashboard
{
  /// <summary>
  /// Data portal server dashboard that reports metrics
  /// using OpenTelemetry instrumentation.
  /// </summary>
  public class OpenTelemetryDashboard : IDashboard
  {
    private readonly Meter _meter;
    private readonly Counter<long> _totalCallsCounter;
    private readonly Counter<long> _completedCallsCounter;
    private readonly Counter<long> _failedCallsCounter;
    private readonly Histogram<double> _callDurationHistogram;

    /// <summary>
    /// Gets the OpenTelemetry meter name for CSLA data portal metrics.
    /// </summary>
    public const string MeterName = "Csla.DataPortal";

    /// <summary>
    /// Gets the OpenTelemetry meter version.
    /// </summary>
    public const string MeterVersion = "1.0.0";

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public OpenTelemetryDashboard()
    {
      _meter = new Meter(MeterName, MeterVersion);

      _totalCallsCounter = _meter.CreateCounter<long>(
        "csla.dataportal.calls.total",
        unit: "{call}",
        description: "Total number of data portal calls");

      _completedCallsCounter = _meter.CreateCounter<long>(
        "csla.dataportal.calls.completed",
        unit: "{call}",
        description: "Number of successfully completed data portal calls");

      _failedCallsCounter = _meter.CreateCounter<long>(
        "csla.dataportal.calls.failed",
        unit: "{call}",
        description: "Number of failed data portal calls");

      _callDurationHistogram = _meter.CreateHistogram<double>(
        "csla.dataportal.call.duration",
        unit: "ms",
        description: "Duration of data portal calls in milliseconds");

      FirstCall = DateTimeOffset.Now;
    }

    /// <summary>
    /// Gets the time the data portal was first invoked
    /// </summary>
    public DateTimeOffset FirstCall { get; }

    /// <summary>
    /// Gets the most recent time the data portal
    /// was invoked
    /// </summary>
    public DateTimeOffset LastCall { get; private set; }

    private long _totalCalls;
    /// <summary>
    /// Gets the total number of times the data portal
    /// has been invoked
    /// </summary>
    public long TotalCalls => Interlocked.Read(ref _totalCalls);

    private long _completedCalls;
    /// <summary>
    /// Gets the number of times data portal
    /// calls have successfully completed
    /// </summary>
    public long CompletedCalls => Interlocked.Read(ref _completedCalls);

    private long _failedCalls;
    /// <summary>
    /// Gets the number of times data portal
    /// calls have failed
    /// </summary>
    public long FailedCalls => Interlocked.Read(ref _failedCalls);

    /// <summary>
    /// Gets the items in the recent activity queue.
    /// </summary>
    public List<Activity> GetRecentActivity()
    {
      return new List<Activity>();
    }

    /// <inheritdoc />
    void IDashboard.InitializeCall(InterceptArgs e)
    {
      if (e is null)
        throw new ArgumentNullException(nameof(e));

      LastCall = DateTimeOffset.Now;
      Interlocked.Add(ref _totalCalls, 1);

      var tags = new TagList
      {
        { "object.type", e.ObjectType.Name },
        { "operation", e.Operation.ToString() }
      };

      _totalCallsCounter.Add(1, tags);
    }

    /// <inheritdoc />
    void IDashboard.CompleteCall(InterceptArgs e)
    {
      if (e is null)
        throw new ArgumentNullException(nameof(e));

      var tags = new TagList
      {
        { "object.type", e.ObjectType.Name },
        { "operation", e.Operation.ToString() }
      };

      if (e.Exception != null)
      {
        Interlocked.Add(ref _failedCalls, 1);
        tags.Add("exception.type", e.Exception.GetType().Name);
        _failedCallsCounter.Add(1, tags);
      }
      else
      {
        Interlocked.Add(ref _completedCalls, 1);
        _completedCallsCounter.Add(1, tags);
      }

      _callDurationHistogram.Record(e.Runtime.TotalMilliseconds, tags);
    }

    /// <summary>
    /// Dispose resources used by this object.
    /// </summary>
    public void Dispose()
    {
      _meter?.Dispose();
    }
  }
}
