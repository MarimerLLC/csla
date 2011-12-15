using System;
using Bxf;
using Csla;

namespace WpUI.ViewModels
{
  public interface IShowStatus
  {
    void ShowStatus(Bxf.Status status);
  }

  /// <summary>
  /// Base viewmodel type for use with model types that are
  /// loaded from the app server (root business types).
  /// </summary>
  public class ViewModel<T> : Csla.Xaml.ViewModelBase<T>, IShowStatus
  {
    public ViewModel()
    {
      Bxf.Shell.Instance.ShowStatus(new Status { IsBusy = true, Text = "Loading..." });
    }

    protected override void OnRefreshed()
    {
      Bxf.Shell.Instance.ShowStatus(new Status());
      base.OnRefreshed();
    }

    protected override void OnError(Exception error)
    {
      Bxf.Shell.Instance.ShowStatus(new Status());
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

      Bxf.Shell.Instance.ShowError(message, "Error");
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
      if (string.IsNullOrWhiteSpace(status.Text))
        StatusContent = null;
      else
        StatusContent = new Views.StatusDisplay { DataContext = status };
    }
  }
}
