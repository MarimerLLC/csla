//-----------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Csla.Threading
{
  /// <summary>
  /// Run an async operation synchronously without
  /// blocking the UI thread.
  /// </summary>
  public static class TaskExtensions
  {
    private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    /// <summary>
    /// Run an async function synchronously without
    /// blocking the UI thread.
    /// </summary>
    /// <remarks>
    /// The async function is run on a thread in the thread
    /// pool, the result is marshalled back to the calling
    /// thread. The calling thread's context is carried
    /// onto the background thread.
    /// </remarks>
    /// <param name="task">Task to run synchronously.</param>
    public static T RunWithContext<T>(this Task<T> task)
    {
      var context = new ContextParams();
      var background = Task.Run(() =>
      {
        context.SetThreadContext();
        task.RunSynchronously();
      });
      SpinWait(background);
      return task.Result;
    }

    /// <summary>
    /// Run an async function synchronously without
    /// blocking the UI thread.
    /// </summary>
    /// <remarks>
    /// The async function is run on a thread in the thread
    /// pool, the result is marshalled back to the calling
    /// thread. The calling thread's context is carried
    /// onto the background thread.
    /// </remarks>
    /// <param name="task">Task to run synchronously.</param>
    /// <param name="timeout">Max time to wait for completion.</param>
    public static T RunWithContext<T>(this Task<T> task, TimeSpan timeout)
    {
      var context = new ContextParams();
      var background = Task.Run(() => 
      {
        context.SetThreadContext();
        task.RunSynchronously();
      });
      SpinWait(background, timeout);
      return task.Result;
    }

    /// <summary>
    /// Wait synchronously (spinwait) for the task to complete.
    /// </summary>
    /// <param name="task">Task on which to wait.</param>
    public static void SpinWait(this Task task)
    {
      while (!task.IsCompleted)
      {
        Thread.Sleep(1);
      }
    }

    /// <summary>
    /// Wait synchronously (spinwait) for the task to complete.
    /// </summary>
    /// <param name="task">Task on which to wait.</param>
    /// <param name="timeout">Timeout value</param>
    public static void SpinWait(this Task task, TimeSpan timeout)
    {
      var deadline = DateTime.Now + timeout;
      while (!task.IsCompleted)
      {
        if (DateTime.Now > deadline)
          throw new TimeoutException();
        Thread.Sleep(1);
      }
    }
  }
}
