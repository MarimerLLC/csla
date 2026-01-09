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
      var t = RefreshAsync<BusinessLibrary.Order>(async () =>
      {
        try
        {
          var portal = App.ApplicationContext.GetRequiredService<IDataPortal<BusinessLibrary.Order>>();
          return await portal.FetchAsync(441);
        }
        catch (Exception ex)
        {
          var fullError = $"Error fetching order:\n{ex.Message}\n\nFull details:\n{ex}";
          try
          {
            Bxf.Shell.Instance.ShowError(fullError, "Data Portal Error");
          }
          catch
          {
            System.Windows.MessageBox.Show(fullError, "Data Portal Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
          }
          return null;
        }
      });
    }

    public override System.Threading.Tasks.Task<Order> SaveAsync()
    {
      try
      {
        return base.SaveAsync();
      }
      catch (Exception ex)
      {
        var x = ex;
        throw;
      }
    }

    public override async Task SaveAsync(object sender, ExecuteEventArgs e)
    {
      try
      {
        await base.SaveAsync(sender, e);
      }
      catch (Exception ex)
      {
        var x = ex;
      }
    }
  }
}
