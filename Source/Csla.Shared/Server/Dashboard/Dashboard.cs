//-----------------------------------------------------------------------
// <copyright file="Dashboard.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Data portal dashboard</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Csla.Server.Dashboard
{
  /// <summary>
  /// Data portal server dashboard.
  /// </summary>
  public class Dashboard : IDashboard
  {
    private readonly object _syncLock = new object();
    private ConcurrentQueue<InterceptArgs> _initializeQueue = new ConcurrentQueue<InterceptArgs>();
    private ConcurrentQueue<InterceptArgs> _completeQueue = new ConcurrentQueue<InterceptArgs>();
    private readonly Timer _timerInitialize;
    private readonly Timer _timerComplete;
    private ConcurrentQueue<Activity> _recentActivity = new ConcurrentQueue<Activity>();
    private const int _timerDueTime = 50;
    private const int _timerPeriod = 500;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public Dashboard()
    {
      _timerInitialize = new Timer(ProcessInitializeQueue, null, _timerDueTime, _timerPeriod);
      _timerComplete = new Timer(ProcessCompleteQueue, null, _timerDueTime, _timerPeriod);
      FirstCall = DateTimeOffset.Now;
    }

    /// <summary>
    /// Gets or sets a value indicating the number of items
    /// to maintain in the recent activity list. Default is 100.
    /// </summary>
    public int RecentActivityCount { get; set; } = 100;

    private void ProcessCompleteQueue(object state)
    {
      if (_completeQueue.IsEmpty)
        return;

      _timerComplete.Change(Timeout.Infinite, Timeout.Infinite);
      try
      {
        while (_completeQueue.TryDequeue(out InterceptArgs result))
        {
          if (result.Exception != null)
            Interlocked.Add(ref _failedCalls, 1);
          else
            Interlocked.Add(ref _completedCalls, 1);
          _recentActivity.Enqueue(new Activity(result));
        }
        // trim list to most recent items
        while (_recentActivity.Count > RecentActivityCount)
          _recentActivity.TryDequeue(out Activity discard);
      }
      finally
      {
        _timerComplete.Change(_timerDueTime, _timerPeriod);
      }
    }

    private void ProcessInitializeQueue(object state)
    {
      if (_initializeQueue.IsEmpty)
        return;

      _timerInitialize.Change(Timeout.Infinite, Timeout.Infinite);
      try
      {
        while (_initializeQueue.TryDequeue(out InterceptArgs result))
        {
          Interlocked.Add(ref _totalCalls, 1);
        }
      }
      finally
      {
        _timerInitialize.Change(_timerDueTime, _timerPeriod);
      }
    }

    /// <summary>
    /// Gets the time the data portal was first invoked
    /// </summary>
    public DateTimeOffset FirstCall { get; private set; }
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
    public long TotalCalls
    {
      get { return Interlocked.Read(ref _totalCalls); }
    }

    private long _completedCalls;
    /// <summary>
    /// Gets the number of times data portal
    /// calls have successfully completed
    /// </summary>
    public long CompletedCalls
    {
      get { return Interlocked.Read(ref _completedCalls); }
    }

    private long _failedCalls;
    /// <summary>
    /// Gets the number of times data portal
    /// calls have failed
    /// </summary>
    public long FailedCalls
    {
      get { return Interlocked.Read(ref _failedCalls); }
    }

    /// <summary>
    /// Gets the items in the recent activity queue.
    /// </summary>
    public List<Activity> GetRecentActivity()
    {
      return _recentActivity.ToList();
    }

    void IDashboard.InitializeCall(InterceptArgs e)
    {
      LastCall = DateTimeOffset.Now;
      _initializeQueue.Enqueue(e);
    }

    void IDashboard.CompleteCall(InterceptArgs e)
    {
      _completeQueue.Enqueue(e);
    }

    /// <summary>
    /// Dispose resources used by this object.
    /// </summary>
    public void Dispose()
    {
      _timerComplete.Dispose();
      _timerInitialize.Dispose();
    }
  }
}
