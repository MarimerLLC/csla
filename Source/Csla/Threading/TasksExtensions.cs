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
        /// Configures the scheduler.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>
        /// Returns a new awaitable task with specified scheduler.
        /// </returns>
        public static CustomTaskAwaitable<T> ConfigureScheduler<T>(this Task<T> task, TaskScheduler scheduler)
        {
            return new CustomTaskAwaitable<T>(task, scheduler);
        }

        #endregion

        /// <summary>
        /// Represents a custom task awaitable.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        public struct CustomTaskAwaitable<T>
        {
            #region Fields

            private readonly CustomTaskAwaiter<T> _awaitable;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomTaskAwaitable{T}"/> struct.
            /// </summary>
            /// <param name="task">The task.</param>
            /// <param name="scheduler">The scheduler.</param>
            public CustomTaskAwaitable(Task<T> task, TaskScheduler scheduler)
            {
                this._awaitable = new CustomTaskAwaiter<T>(task, scheduler);
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// Gets the awaiter.
            /// </summary>
            /// <returns>
            /// Returns the awaiter.
            /// </returns>
            public CustomTaskAwaiter<T> GetAwaiter()
            {
                return this._awaitable;
            }

            #endregion

            /// <summary>
            /// Represents a custom task awaiter
            /// </summary>
            /// <typeparam name="TResult">Type of the result</typeparam>
            public struct CustomTaskAwaiter<TResult> : INotifyCompletion
            {
                #region Fields

                private readonly Task<TResult> _task;
                private readonly TaskScheduler _scheduler;

                #endregion

                #region Constructors and Destructors

                /// <summary>
                /// Initializes a new instance of the <see cref="CustomTaskAwaiter`1"/> struct.
                /// </summary>
                /// <param name="task">The task.</param>
                /// <param name="scheduler">The scheduler.</param>
                public CustomTaskAwaiter(Task<TResult> task, TaskScheduler scheduler)
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
                /// <returns></returns>
                public TResult GetResult()
                {
                    return this._task.Result;
                }

                #endregion
            }
        }
    }
}