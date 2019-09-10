using System;
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
          return await DataPortal.FetchAsync<BusinessLibrary.Order>(441);
        }
        catch (Exception ex)
        {
          Bxf.Shell.Instance.ShowError(ex.Message, "Error");
          return null;
        }
      });
    }
  }
}
