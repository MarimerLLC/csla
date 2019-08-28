using System;

namespace WpfUI.ViewModels
{
  /// <summary>
  /// Base viewmodel type for use with editable model types that are
  /// loaded from the app server (editable root business types).
  /// </summary>
  public class ViewModelEdit<T> : ViewModel<T>
    where T : Csla.Core.ITrackStatus
  {
    public void Cancel()
    {
      base.DoCancel();
    }

    public async void Save()
    {
      if (Model != null)
      {
        if (Model.IsSavable)
        {
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
          await SaveAsync();
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = true, Text = "Saved..." });
        }
        else
        {
          Bxf.Shell.Instance.ShowError("Object can not be saved", "Save error");
        }
      }
    }
  }
}
