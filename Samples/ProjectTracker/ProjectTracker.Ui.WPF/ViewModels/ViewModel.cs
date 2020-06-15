using System;
using System.Threading.Tasks;
using Bxf;
using Csla;

namespace WpfUI.ViewModels
{
  /// <summary>
  /// Base viewmodel type for use with model types that are
  /// loaded from the app server (root business types).
  /// </summary>
  public class ViewModel<T> : Csla.Xaml.ViewModelBase<T>
  {
    protected override async Task<T> RefreshAsync<F>(Func<Task<T>> factory)
    {
      Shell.Instance.ShowStatus(new Status { IsBusy = true, Text = "Loading..." });
      T result = default;
      try
      {
        result = await base.RefreshAsync<F>(factory);
        Shell.Instance.ShowStatus(new Status { IsOk = true });
      }
      catch (Exception ex)
      {
        Shell.Instance.ShowStatus(new Status { IsOk = false });
        string message;
        if (ex is Csla.DataPortalException be)
          message = be.BusinessExceptionMessage;
        else
          message = ex.Message;

        Shell.Instance.ShowError(message, "Error");
      }
      return result;
    }
  }
}
