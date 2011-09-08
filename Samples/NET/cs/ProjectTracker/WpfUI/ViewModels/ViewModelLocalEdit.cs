using System;

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
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
      base.BeginSave();
    }

    protected override void OnSaved()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = true, Text = "Saved..." });
      base.OnSaved();
    }
  }
}
