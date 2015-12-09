//-----------------------------------------------------------------------
// <copyright file="CslaTaskSheduler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implementation of a lock that waits while</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if NETFX_CORE && !NETCORE && !PCL46
using Windows.System.Threading;
#endif

namespace Csla.Threading
{
  /// <summary>
  /// A TaskScheduler that sets CSLA context before task is executed.
  /// </summary>
  public class CslaTaskScheduler : TaskScheduler
  {
    /// <summary>Whether the current thread is processing work items.</summary>
    [ThreadStatic]
    private static bool _currentThreadIsProcessingItems;

    private readonly ContextParams _context = new ContextParams();

    /// <summary>The list of tasks to be executed.</summary>
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)

    /// <summary>Queues a task to the scheduler.</summary>
    /// <param name="task">The task to be queued.</param>
    protected override sealed void QueueTask(Task task)
    {
      // Add the task to the list of tasks to be processed.  If there aren't enough
      // delegates currently queued or running to process tasks, schedule another.
      lock (_tasks)
      {
        _tasks.AddLast(task);
        NotifyThreadPoolOfPendingWork();
      }
    }

    /// <summary>
    /// Informs the ThreadPool that there's work to be executed for this scheduler.
    /// </summary>
#if NETFX_CORE && !NETCORE
    private void NotifyThreadPoolOfPendingWork()
    {
#if !PCL46 // rely on NuGet bait-and-switch for actual implementation
      var asyncAction = ThreadPool.RunAsync(_ =>
      {
        // Note that the current thread is now processing work items.
        // This is necessary to enable inlining of tasks into this thread.
        _currentThreadIsProcessingItems = true;
        _context.SetThreadContext();
        try
        {
          // Process all available items in the queue.
          while (true)
          {
            Task item;
            lock (_tasks)
            {
              // When there are no more items to be processed,
              // note that we're done processing, and get out.
              if (_tasks.Count == 0)
              {
                break;
              }

              // Get the next item from the queue
              item = _tasks.First.Value;
              _tasks.RemoveFirst();
            }

            // Execute the task we pulled out of the queue
            TryExecuteTask(item);
          }
        }
        // We're done processing items on the current thread
        finally
        {
          _currentThreadIsProcessingItems = false;
        }
      }, WorkItemPriority.Normal, WorkItemOptions.None);
#endif
    }
#elif (ANDROID || IOS || NETCORE)
    private void NotifyThreadPoolOfPendingWork()
    {
      ThreadPool.QueueUserWorkItem(_ =>
      {
        // Note that the current thread is now processing work items.
        // This is necessary to enable inlining of tasks into this thread.
        _currentThreadIsProcessingItems = true;
        _context.SetThreadContext();
        try
        {
          // Process all available items in the queue.
          while (true)
          {
            Task item;
            lock (_tasks)
            {
              // When there are no more items to be processed,
              // note that we're done processing, and get out.
              if (_tasks.Count == 0)
              {
                break;
              }

              // Get the next item from the queue
              item = _tasks.First.Value;
              _tasks.RemoveFirst();
            }

            // Execute the task we pulled out of the queue
            base.TryExecuteTask(item);
          }
        }
        // We're done processing items on the current thread
        finally
        {
          _currentThreadIsProcessingItems = false;
        }
      }, null);
    }
#else
    private void NotifyThreadPoolOfPendingWork()
    {
      ThreadPool.UnsafeQueueUserWorkItem(_ =>
      {
        // Note that the current thread is now processing work items.
        // This is necessary to enable inlining of tasks into this thread.
        _currentThreadIsProcessingItems = true;
        _context.SetThreadContext();
        try
        {
          // Process all available items in the queue.
          while (true)
          {
            Task item;
            lock (_tasks)
            {
              // When there are no more items to be processed,
              // note that we're done processing, and get out.
              if (_tasks.Count == 0)
              {
                break;
              }

              // Get the next item from the queue
              item = _tasks.First.Value;
              _tasks.RemoveFirst();
            }

            // Execute the task we pulled out of the queue
            base.TryExecuteTask(item);
          }
        }
        // We're done processing items on the current thread
        finally
        {
          _currentThreadIsProcessingItems = false;
        }
      }, null);
    }
#endif
    /// <summary>Attempts to execute the specified task on the current thread.</summary>
    /// <param name="task">The task to be executed.</param>
    /// <param name="taskWasPreviouslyQueued"></param>
    /// <returns>Whether the task could be executed on the current thread.</returns>
    protected override sealed bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
      // If this thread isn't already processing a task, we don't support inlining
      if (!_currentThreadIsProcessingItems) return false;

      // If the task was previously queued, remove it from the queue
      if (taskWasPreviouslyQueued) TryDequeue(task);

      // Try to run the task.
      return base.TryExecuteTask(task);
    }

    /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
    /// <param name="task">The task to be removed.</param>
    /// <returns>Whether the task could be found and removed.</returns>
    protected override sealed bool TryDequeue(Task task)
    {
      lock (_tasks) return _tasks.Remove(task);
    }

    /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
    /// <returns>An enumerable of the tasks currently scheduled.</returns>
    protected override sealed IEnumerable<Task> GetScheduledTasks()
    {
      bool lockTaken = false;
      try
      {
        Monitor.TryEnter(_tasks, ref lockTaken);
        if (lockTaken) return _tasks.ToArray();
        else throw new NotSupportedException();
      }
      finally
      {
        if (lockTaken) Monitor.Exit(_tasks);
      }
    }
  }
}
