using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Csla.Serialization;

namespace Csla.Core.LoadManager
{
  internal class AsyncLoadManager : INotifyBusy
  {
    private ObservableCollection<AsyncLoader> _loading = new ObservableCollection<AsyncLoader>();

    public AsyncLoadManager() { }

    public bool IsLoading
    {
      get
      {
        lock (_loading)
          return _loading.Count > 0;
      }
    }

    public void BeginLoad(AsyncLoader loader, Delegate complete)
    {
      bool isAlreadyBusy = false;
      lock(_loading)
      {
        isAlreadyBusy = (from l in _loading
                         where l.Property == loader.Property
                         select l).Count() > 0;
        _loading.Add(loader);
      }

      loader.Complete += new EventHandler<ErrorEventArgs>(loader_Complete);

      if(!isAlreadyBusy)
        OnPropertyBusy(loader.Property.Name, true);
      
      loader.Load(complete);
    }

    void loader_Complete(object sender, ErrorEventArgs e)
    {
      AsyncLoader loader = (AsyncLoader)sender;
      loader.Complete -= new EventHandler<ErrorEventArgs>(loader_Complete);

      bool isStillBusy = false;
      lock (_loading)
      {
        _loading.Remove(loader);
        isStillBusy = (from l in _loading
                       where l.Property == loader.Property
                       select l).Count() > 0;
      }

      if (!isStillBusy)
        OnPropertyBusy(loader.Property.Name, false);

      if (e.Error != null)
        OnUnhandledAsyncException(this, e.Error);
    }

    #region INotifyPropertyBusy Members

    public event BusyChangedEventHandler BusyChanged;
    protected void OnPropertyBusy(string propertyName, bool busy)
    {
      if (BusyChanged != null)
        BusyChanged(this, new BusyChangedEventArgs(propertyName, busy));
    }

    #endregion

    #region INotifyBusy Members

    bool INotifyBusy.IsBusy
    {
      get { return (this as INotifyBusy).IsSelfBusy; }
    }

    bool INotifyBusy.IsSelfBusy
    {
      get { return IsLoading; }
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Remove(_unhandledAsyncException, value); }
    }

    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

    #endregion
  }
}
