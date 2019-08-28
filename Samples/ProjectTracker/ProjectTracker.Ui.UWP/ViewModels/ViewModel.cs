using System;
using System.Threading.Tasks;
using Csla.Xaml;
using Windows.UI.Popups;

namespace UwpUI.ViewModels
{
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    protected override async Task<T> RefreshAsync<F>(Func<Task<T>> factory)
    {
      T result = default;
      try
      {
        result = await base.RefreshAsync<F>(factory);
      }
      catch (Exception ex)
      {
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
        await new MessageDialog(message).ShowAsync();
      }
      return result;
    }
  }
}
