//-----------------------------------------------------------------------
// <copyright file="INotifyBusy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining an object that notifies when it</summary>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Csla.Core {
  /// <summary>
  /// Helper class for busy related functionality spread across different business type implementations.
  /// </summary>
  public static class BusyHelper 
  {
    /// <summary>
    /// Helper class method for busy related functionality spread across different business type implementations.
    /// </summary>
    public static async Task WaitForIdle(INotifyBusy source, TimeSpan timeout, [CallerMemberName] string methodName = "") 
    {
      if (!source.IsBusy) 
      {
        return;
      }

      var tcs = new TaskCompletionSource<object>();
      try 
      {
        source.BusyChanged += ObserverForIsBusyChange;

        if (!source.IsBusy) 
        {
          return;
        }

        var timeoutTask = Task.Delay(timeout);
        var finishedTask = await Task.WhenAny(tcs.Task, timeoutTask).ConfigureAwait(false);

        if (finishedTask == timeoutTask)
        {
          throw new TimeoutException($"{source.GetType().FullName}.{methodName} - {timeout}.");
        }
      }
      finally 
      {
        source.BusyChanged -= ObserverForIsBusyChange;
      }

      void ObserverForIsBusyChange(object sender, BusyChangedEventArgs e)
      {
        if (!source.IsBusy && !e.Busy) 
        {
          tcs.TrySetResult(null);
        }
      }
    }
  }
}