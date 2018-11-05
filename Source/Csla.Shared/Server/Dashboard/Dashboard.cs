//-----------------------------------------------------------------------
// <copyright file="Dashboard.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Data portal dashboard</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Concurrent;
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
    private readonly Timer _timer;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public Dashboard()
    {
      _timer = new Timer(ProcessQueues, null, 100, 1000);
      FirstCall = DateTimeOffset.Now;
    }

    private void ProcessQueues(object state)
    {
      if (_initializeQueue.IsEmpty && _completeQueue.IsEmpty)
        return;

      _timer.Change(Timeout.Infinite, Timeout.Infinite);
      try
      {
        while (_initializeQueue.TryDequeue(out InterceptArgs result))
        {
          Interlocked.Add(ref _totalCalls, 1);
        }

        while (_completeQueue.TryDequeue(out InterceptArgs result))
        {
          if (result.Exception != null)
            Interlocked.Add(ref _failedCalls, 1);
          else
            Interlocked.Add(ref _completedCalls, 1);
        }
      }
      finally
      {
        _timer.Change(100, 1000);
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
    /// Gets the completed number of times the data portal
    /// has been invoked
    /// </summary>
    public long CompletedCalls
    {
      get { return Interlocked.Read(ref _completedCalls); }
    }

    private long _failedCalls;
    /// <summary>
    /// Gets the failed number of times the data portal
    /// has been invoked
    /// </summary>
    public long FailedCalls
    {
      get { return Interlocked.Read(ref _failedCalls); }
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
  }
}
