//-----------------------------------------------------------------------
// <copyright file="Semaphore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a semaphore style lock</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;

namespace Csla.Threading
{
  /// <summary>
  /// Implements a semaphore style lock
  /// using a busy wait technique (user mode).
  /// </summary>
  public class Semaphore : IDisposable
  {
    private static object _lock = typeof(Semaphore); // Use the type to lock across threads (if SL respects this...)
    private static int _active;
    private static int _max = 1;
    /// <summary>
    /// Gets or sets the max value.
    /// </summary>
    public static int Max
    {
      get { return _max; }
      set
      {
        lock (_lock)
          _max = value;
      }
    }

    /// <summary>
    /// Gets a the number of active locks.
    /// </summary>
    public static int Active
    {
      get { return _active; }
    }

    /// <summary>
    /// Creates an instance of the object,
    /// placing a lock on the semaphore,
    /// blocking the caller if the active
    /// locks is at the Max value.
    /// </summary>
    public Semaphore()
    {
      bool okToRun = false;
      do
      {
        if (_active < _max)
        {
          lock (_lock)
          {
            if (_active < _max)
            {
              _active++;
              okToRun = true;
            }
          }
        }
        else Thread.Sleep(2);
      }
      while (!okToRun);
    }

    /// <summary>
    /// Disposes the object, releasing
    /// a lock on the semaphore.
    /// </summary>
    public void Dispose()
    {
      lock (_lock)
        _active--;
    }
  }
}