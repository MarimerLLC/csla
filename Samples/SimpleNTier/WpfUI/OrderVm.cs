using System;
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
      // in the Exception property and raise the Error event.
      Error += (o, e) => ShowError(e.Error);

      _ = RefreshAsync(async () =>
      {
        var portal = App.ApplicationContext.GetRequiredService<IDataPortal<BusinessLibrary.Order>>();
        return await portal.FetchAsync(441);
      });
    }

    private static void ShowError(Exception ex)
    {
      var fullError = $"Data portal error:\n{ex.Message}\n\nFull details:\n{ex}";
      try
      {
        Bxf.Shell.Instance.ShowError(fullError, "Data Portal Error");
      }
      catch
      {
        System.Windows.MessageBox.Show(fullError, "Data Portal Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }
    }
  }
}
