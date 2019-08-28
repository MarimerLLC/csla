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

    public async void Save()
    {
      Shell.Instance.ShowStatus(new Status { IsBusy = true, Text = "Saving..." });
      try
      {
        await SaveAsync();
        Shell.Instance.ShowStatus(new Status { IsOk = true, Text = "Saved..." });
      }
      catch (Exception ex)
      {
        Shell.Instance.ShowStatus(new Status { IsOk = false });
        string message;
        if (ex is Csla.DataPortalException be)
        {
          if (be.BusinessException != null)
            message = be.BusinessException.Message;
          else
            message = be.Message;
        }
        else
          message = ex.Message;

        Shell.Instance.ShowError(message, "Error");
      }
    }
  }
}
