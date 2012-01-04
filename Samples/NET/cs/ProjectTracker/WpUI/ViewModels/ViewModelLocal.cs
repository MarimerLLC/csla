using System;
using Bxf;

namespace WpUI.ViewModels
{
  /// <summary>
  /// Base viewmodel type for use with model types that are
  /// NOT loaded from the app server (either child business
  /// types, or a viewmodel created from an already-loaded
  /// object).
  /// </summary>
  public class ViewModelLocal<T> : Csla.Xaml.ViewModelBase<T>, IShowStatus, IViewModel
  {
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

    public virtual void NavigatingTo()
    { }

    public virtual void NavigatedAway()
    { }
  }
}
