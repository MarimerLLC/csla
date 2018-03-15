using System;
using Csla.Xaml;
using Windows.UI.Popups;

namespace UwpUI.ViewModels
{
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    protected async override void OnError(Exception error)
    {
      base.OnError(error);
      string message = error.Message;
      if (error.InnerException != null)
        message = error.InnerException.Message;
      //await new MessageDialog(message, $"Data error { typeof(T) }").ShowAsync();
    }
  }
}
