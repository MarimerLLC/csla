using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;
using Windows.UI.Popups;

namespace WinRtUI.ViewModel
{
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    protected async override void OnError(Exception error)
    {
      base.OnError(error);
      string message = error.Message;
      if (error.InnerException != null)
        message = error.InnerException.Message;
      await new MessageDialog(message, "Data error").ShowAsync();
    }
  }
}
