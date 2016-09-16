#if NETFX_CORE && !WINDOWS_UWP && !PCL46 && !NETSTANDARD
//-----------------------------------------------------------------------
// <copyright file="BackgroundWorkerBCL.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implementation of old BCL BackgroundWorker ported to WinRT.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace System.ComponentModel
{
  /// <summary>
  /// Implementation of a BackgroundWorker using
  /// the async/await keywords.
  /// </summary>
  public class BackgroundWorker : DependencyObject
  {
    private CoreDispatcher _dispatcher;

    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    public BackgroundWorker()
    {
      _dispatcher = this.Dispatcher;
    }

    /// <summary>
    /// Requests that the background task
    /// cancel its operation.
    /// </summary>
    public void CancelAsync()
    {
      if (!WorkerSupportsCancellation)
        throw new NotSupportedException();
      CancellationPending = true;
    }

    /// <summary>
    /// Gets a value indicating whether a cancel
    /// request is pending.
    /// </summary>
    public bool CancellationPending { get; private set; }

    /// <summary>
    /// Event raised on the UI thread to indicate that
    /// progress has changed.
    /// </summary>
    public event ProgressChangedEventHandler ProgressChanged;

    /// <summary>
    /// Call from the worker thread to report on progress.
    /// </summary>
    /// <param name="percentProgress">Percent complete.</param>
    public void ReportProgress(int percentProgress)
    {
      ReportProgress(percentProgress, null);
    }

    /// <summary>
    /// Call from the worker thread to report on progress.
    /// </summary>
    /// <param name="percentProgress">Percent complete.</param>
    /// <param name="userState">User state value.</param>
    public async void ReportProgress(int percentProgress, object userState)
    {
      if (ProgressChanged != null)
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,
          () =>
          {
            ProgressChanged(this, new ProgressChangedEventArgs(percentProgress, userState));
          });
    }

    /// <summary>
    /// Gets or sets a value indicating whether the worker 
    /// reports progress.
    /// </summary>
    public bool WorkerReportsProgress { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the worker 
    /// supports cancellation.
    /// </summary>
    public bool WorkerSupportsCancellation { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the worker 
    /// is currently executing.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Event raised on a background thread when work is to be
    /// performed. The code handling this event should implement
    /// the background task.
    /// </summary>
    public event DoWorkEventHandler DoWork;
    /// <summary>
    /// Event raised on the UI thread when work is complete.
    /// </summary>
    public event RunWorkerCompletedEventHandler RunWorkerCompleted;
    /// <summary>
    /// Raise the RunWorkerCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
    {
      if (RunWorkerCompleted != null)
        RunWorkerCompleted(this, e);
    }

    /// <summary>
    /// Starts the background task by raising the DoWork event.
    /// </summary>
    public void RunWorkerAsync()
    {
      RunWorkerAsync(null);
    }

    /// <summary>
    /// Starts the background task by raising the DoWork event.
    /// </summary>
    /// <param name="userState">User state value.</param>
    public async void RunWorkerAsync(object userState)
    {
      if (DoWork != null)
      {
        CancellationPending = false;
        IsBusy = true;
        try
        {
          var args = new DoWorkEventArgs { Argument = userState };
          await Task.Run(() =>
          {
            DoWork(this, args);
          });
          IsBusy = false;
          OnRunWorkerCompleted(new RunWorkerCompletedEventArgs { Result = args.Result });
        }
        catch (Exception ex)
        {
          IsBusy = false;
          OnRunWorkerCompleted(new RunWorkerCompletedEventArgs { Error = ex });
        }
      }
    }
  }

  /// <summary>
  /// DoWork method definition.
  /// </summary>
  /// <param name="sender">Sender.</param>
  /// <param name="e">Event arguments.</param>
  public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs e);

  /// <summary>
  /// Event arguments passed to the DoWork event/method.
  /// </summary>
  public class DoWorkEventArgs : EventArgs
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public DoWorkEventArgs()
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="argument">Argument passed to DoWork handler.</param>
    public DoWorkEventArgs(object argument)
    {
      Argument = argument;
    }

    /// <summary>
    /// Gets or sets the argument value passed into
    /// the DoWork handler.
    /// </summary>
    public object Argument { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// operation was cancelled prior to completion.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets or sets a value containing the result
    /// of the operation.
    /// </summary>
    public object Result { get; set; }
  }

  /// <summary>
  /// RunWorkerCompleted method definition.
  /// </summary>
  /// <param name="sender">Sender.</param>
  /// <param name="e">Event arguments.</param>
  public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);

  /// <summary>
  /// Event arguments passed to the RunWorkerCompleted handler.
  /// </summary>
  public class RunWorkerCompletedEventArgs : EventArgs
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public RunWorkerCompletedEventArgs()
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="cancelled">Sets the cancelled value.</param>
    /// <param name="error">Sets the error value.</param>
    /// <param name="result">Sets the result value.</param>
    public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
    {
      Result = result;
      Error = error;
      Cancelled = cancelled;
    }

    /// <summary>
    /// Gets or sets a value containing any exception
    /// that terminated the background task.
    /// </summary>
    public Exception Error { get; set; }
    /// <summary>
    /// Gets or sets a value containing the result
    /// of the operation.
    /// </summary>
    public object Result { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// operation was cancelled prior to completion.
    /// </summary>
    public bool Cancelled { get; set; }
  }
}
#endif