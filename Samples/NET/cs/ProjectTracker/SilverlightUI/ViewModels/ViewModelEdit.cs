using System;

namespace SilverlightUI.ViewModels
{
  public class ViewModelEdit<T> : ViewModel<T>
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
