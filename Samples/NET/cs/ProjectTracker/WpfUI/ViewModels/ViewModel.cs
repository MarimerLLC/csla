using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bxf;

namespace WpfUI.ViewModels
{
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
  }
}
