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
      var t = RefreshAsync<BusinessLibrary.Order>(async () =>
      {
        try
        {
          var portal = App.ApplicationContext.GetRequiredService<IDataPortal<BusinessLibrary.Order>>();
          return await portal.FetchAsync(441);
        }
        catch (Exception ex)
        {
          Bxf.Shell.Instance.ShowError(ex.Message, "Error");
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

    public override void SaveAsync(object sender, ExecuteEventArgs e)
    {
      try
      {
        base.SaveAsync(sender, e);
      }
      catch (Exception ex)
      {
        var x = ex;
      }
    }
  }
}
