using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Xaml;

namespace WpUI
{
  public class OrderVm : ViewModel<BusinessLibrary.Order>
  {
    public OrderVm()
    {
      Header = "order";
      //BeginRefresh(BusinessLibrary.Order.NewOrder);
      BeginRefresh(handler => BusinessLibrary.Order.GetOrder(441, handler));
    }

    protected override void OnError(Exception error)
    {
      Bxf.Shell.Instance.ShowError(error.Message, "Error");
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
      Bxf.Shell.Instance.ShowView(
        typeof(LineItemList).AssemblyQualifiedName,
        "lineItemListVmViewSource",
        new LineItemListVm(Model),
        "2");
    }
  }
}
