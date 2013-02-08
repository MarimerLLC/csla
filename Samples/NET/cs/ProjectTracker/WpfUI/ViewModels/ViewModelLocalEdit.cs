using System;
using Bxf;

namespace WpfUI.ViewModels
{
  /// <summary>
  /// Base viewmodel type for use with editable model types that are
  /// NOT loaded from the app server (editable root objects).
  /// </summary>
  public class ViewModelLocalEdit<T> : ViewModelLocal<T>
  {
    public void Cancel()
    {
      base.DoCancel();
    }

    public void Save()
    {
      Bxf.Shell.Instance.ShowStatus(new Status { IsBusy = true, Text = "Saving..." });
      base.BeginSave();
    }

    protected override void OnSaved()
    {
      Bxf.Shell.Instance.ShowStatus(new Status { IsOk = true, Text = "Saved..." });
      base.OnSaved();
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
