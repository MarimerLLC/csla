using System;
using Bxf;
using Csla;

namespace WpfUI.ViewModels
{
  /// <summary>
  /// Base viewmodel type for use with model types that are
  /// loaded from the app server (root business types).
  /// </summary>
  public class ViewModel<T> : Csla.Xaml.ViewModelBase<T>
  {
    public ViewModel()
    {
      Bxf.Shell.Instance.ShowStatus(new Status { IsBusy = true, Text = "Loading..." });
    }

    protected override void OnRefreshed()
    {
      Bxf.Shell.Instance.ShowStatus(new Status { IsOk = true });
      base.OnRefreshed();
    }

    protected override void OnError(Exception error)
    {
      Bxf.Shell.Instance.ShowStatus(new Status { IsOk = false });
      string message = null;
      var be = error as Csla.DataPortalException;
      if (be != null)
      {
        if (be.BusinessException != null)
          message = be.BusinessException.Message;
        else
          message = be.Message;
      }
      else
        message = error.Message;

      Bxf.Shell.Instance.ShowError(message, "Error");
      base.OnError(error);
    }
  }
}
