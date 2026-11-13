using System;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Csla.Xaml;

namespace WpfUI
{
  public class OrderVm : ViewModel<BusinessLibrary.Order>
  {
    public OrderVm()
    {
      // RefreshAsync and SaveAsync don't throw. They capture the exception
      // in the Exception property and raise the Error event. Error is also
      // raised when an authorization rule fails.
      Error += (o, e) => ShowError(ViewModelErrorText, e.Error.ToString());

      _ = RefreshAsync(async () =>
      {
        var portal = App.ApplicationContext.GetRequiredService<IDataPortal<BusinessLibrary.Order>>();
        return await portal.FetchAsync(441);
      });
    }

    public override async Task SaveAsync(object sender, ExecuteEventArgs e)
    {
      await base.SaveAsync(sender, e);
      // An invalid order isn't saved. That sets ViewModelErrorText,
      // but not Exception, and doesn't raise Error.
      if (Exception is null && Model is { IsValid: false })
      {
        var message = ViewModelErrorText;
        if (string.IsNullOrEmpty(message))
          message = "The order isn't valid, so it wasn't saved.";
        ShowError(message, null);
      }
    }

    private static void ShowError(string message, string details)
    {
      var text = details is null ? message : $"{message}\n\nFull details:\n{details}";
      try
      {
        Bxf.Shell.Instance.ShowError(text, "Error");
      }
      catch
      {
        System.Windows.MessageBox.Show(text, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }
    }
  }
}
