using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Xaml;

namespace WpfUI
{
  public class OrderVm : ViewModel<BusinessLibrary.Order>
  {
    public OrderVm()
    {
      //DoRefresh(BusinessLibrary.Order.NewOrder);
      //DoRefresh(() => BusinessLibrary.Order.GetOrder(441));
      //BeginRefresh(BusinessLibrary.Order.NewOrder);
      //BeginRefresh(callback => BusinessLibrary.Order.GetOrder(441, callback));
    }

    protected async override System.Threading.Tasks.Task<BusinessLibrary.Order> DoInitAsync()
    {
      return await BusinessLibrary.Order.GetOrderAsync(441);
    }

    protected override void OnError(Exception error)
    {
      Bxf.Shell.Instance.ShowError(error.Message, "Error");
    }
  }
}
