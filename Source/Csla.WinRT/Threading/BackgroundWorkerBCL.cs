//-----------------------------------------------------------------------
// <copyright file="BackgroundWorker.cs" company="Marimer LLC">
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
using Windows.UI.Xaml;

namespace System.ComponentModel
{
  public class BackgroundWorker : DependencyObject
  {
    private CoreDispatcher _dispatcher;

    public BackgroundWorker()
    {
      _dispatcher = this.Dispatcher;
    }

    public void CancelAsync()
    {
      if (!WorkerSupportsCancellation)
        throw new NotSupportedException();
      CancellationPending = true;
    }

    public bool CancellationPending { get; private set; }

    public event ProgressChangedEventHandler ProgressChanged;

    public void ReportProgress(int percentProgress)
    {
      ReportProgress(percentProgress, null);
    }

    public void ReportProgress(int percentProgress, object userState)
    {
      if (ProgressChanged != null)
        _dispatcher.Invoke(CoreDispatcherPriority.Normal,
          (sender, args) =>
          {
            ProgressChanged(this, new ProgressChangedEventArgs(percentProgress, userState));
          },
          this, null);
    }

    public bool WorkerReportsProgress { get; set; }
    public bool WorkerSupportsCancellation { get; set; }
    public bool IsBusy { get; set; }

    public event DoWorkEventHandler DoWork;
    public event RunWorkerCompletedEventHandler RunWorkerCompleted;
    protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
    {
      if (RunWorkerCompleted != null)
        RunWorkerCompleted(this, e);
    }

    public void RunWorkerAsync()
    {
      RunWorkerAsync(null);
    }

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

  public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs e);

  public class DoWorkEventArgs : EventArgs
  {
    public object Argument { get; set; }
    public bool Cancel { get; set; }
    public object Result { get; set; }
  }

  public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);

  public class RunWorkerCompletedEventArgs : EventArgs
  {
    public Exception Error { get; set; }
    public object Result { get; set; }
  }
}
