using System;
using System.Threading.Tasks;
using Csla;
using Csla.Xaml;

namespace XamarinFormsUi.ViewModels
{
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    private string _errorText;
    public string ErrorText
    {
      get { return _errorText; }
      set
      {
        _errorText = value;
        OnPropertyChanged("ErrorText");
      }
    }

    protected override async Task<T> RefreshAsync<F>(Func<Task<T>> factory)
    {
      T result = default;
      try
      {
        result = await base.RefreshAsync<F>(factory);
      }
      catch (DataPortalException ex)
      {
        ErrorText = ex.BusinessExceptionMessage;
      }
      catch (Exception ex)
      {
        ErrorText = ex.Message;
      }
      return result;
    }
  }
}
