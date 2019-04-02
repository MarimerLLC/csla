//-----------------------------------------------------------------------
// <copyright file="SyncTask.cs" company="Marimer LLC">
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
  public static class SyncTask
  {
    private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    /// <summary>
    /// Run an async function synchronously without
    /// blocking the UI thread.
    /// </summary>
    public static T Run<T>(Func<Task<T>> func)
    {
      var context = new ContextParams();
      return _myTaskFactory.StartNew<Task<T>>(delegate
      {
        context.SetThreadContext();
        return func();
      }).Unwrap<T>().GetAwaiter().GetResult();
    }
  }
}
