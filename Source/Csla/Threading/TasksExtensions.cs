using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Threading
{
  /// <summary>
  /// Represents extensions methods for Tasks.
  /// </summary>
  public static class TaskExtensions
  {
    #region Public Methods and Operators

    /// <summary>
    /// Configures the task with a task scheduler.
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <returns>
    /// Returns a new awaitable task with a specified task scheduler.
    /// </returns>
    public static TaskSchedulerAwaitable<T> ConfigureScheduler<T>(this Task<T> task, TaskScheduler scheduler)
    {
      return new TaskSchedulerAwaitable<T>(task, scheduler);
    }

    #endregion

    /// <summary>
    /// Represents a task scheduler awaitable.
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    public struct TaskSchedulerAwaitable<T>
    {
      #region Fields

      private readonly TaskSchedulerAwaiter<T> _awaiter;

      #endregion

      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="TaskSchedulerAwaitable{T}"/> struct.
      /// </summary>
      /// <param name="task">The task.</param>
      /// <param name="scheduler">The scheduler.</param>
      public TaskSchedulerAwaitable(Task<T> task, TaskScheduler scheduler)
      {
        this._awaiter = new TaskSchedulerAwaiter<T>(task, scheduler);
      }

      #endregion

      #region Public Methods and Operators

      /// <summary>
      /// Gets the awaiter.
      /// </summary>
      /// <returns>
      /// Returns the awaiter.
      /// </returns>
      public TaskSchedulerAwaiter<T> GetAwaiter()
      {
        return this._awaiter;
      }

      #endregion

      /// <summary>
      /// Represents a task scheduler awaiter
      /// </summary>
      /// <typeparam name="TResult">Type of the result</typeparam>
      public struct TaskSchedulerAwaiter<TResult> : INotifyCompletion
      {
        #region Fields

        private readonly Task<TResult> _task;
        private readonly TaskScheduler _scheduler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSchedulerAwaiter{TResult}"/> struct.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="scheduler">The scheduler.</param>
        public TaskSchedulerAwaiter(Task<TResult> task, TaskScheduler scheduler)
        {
          this._task = task;
          this._scheduler = scheduler;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance is completed.
        /// </summary>
        public bool IsCompleted
        {
          get
          {
            return this._task.IsCompleted;
          }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        public void OnCompleted(Action continuation)
        {
          // ContinueWith sets the scheduler to use for the continuation action
          this._task.ContinueWith(x => continuation(), this._scheduler);
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns>Returns the result.</returns>
        public TResult GetResult()
        {
          return this._task.Result;
        }

        #endregion
      }
    }
  }
}