using System;
using Csla.Xaml;

namespace XamarinFormsUi.ViewModels
{
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    protected override void OnError(Exception error)
    {
      base.OnError(error);
      string message = error.Message;
      if (error.InnerException != null)
        message = error.InnerException.Message;
      //TODO: display error notification here
      //await new MessageDialog(message, "Data error").ShowAsync();
    }
  }
}
