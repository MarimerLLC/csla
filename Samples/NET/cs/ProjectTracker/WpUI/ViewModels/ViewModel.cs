using System;
using Bxf;
using Csla;

namespace WpUI.ViewModels
{
  /// <summary>
  /// Base viewmodel type for use with model types that are
  /// loaded from the app server (root business types).
  /// </summary>
  public class ViewModel<T> : Csla.Xaml.ViewModelBase<T>, IShowStatus
  {
    public ViewModel()
    {
      var s = new Status { IsBusy = true, Text = "Loading..." };
      App.ViewModel.ShowStatus(s);

      // also directly set the status on this viewmodel, because it
      // can't be the current viewmodel yet while this ctor is running
      ShowStatus(s);
    }

    protected override void OnRefreshed()
    {
      App.ViewModel.ShowStatus(new Status());
      base.OnRefreshed();
    }

    protected override void OnError(Exception error)
    {
      App.ViewModel.ShowStatus(new Status());
      string message = null;
      var be = error as Csla.DataPortalException;
      if (be != null)
      {
        if (be.ErrorInfo != null)
          message = be.ErrorInfo.Message;
        else if (be.InnerException != null)
          message = be.InnerException.Message;
        else
          message = be.Message;
      }
      else
        message = error.Message;

      App.ViewModel.ShowError(message, "Error");
      base.OnError(error);
    }

    private Views.StatusDisplay _statusDisplay;
    public Views.StatusDisplay StatusContent
    {
      get { return _statusDisplay; }
      set { _statusDisplay = value; OnPropertyChanged("StatusContent"); }
    }

    public void ShowStatus(Status status)
    {
      if (status.IsBusy)
        StatusContent = new Views.StatusDisplay { DataContext = status };
      else
        StatusContent = null;
    }
  }
}
