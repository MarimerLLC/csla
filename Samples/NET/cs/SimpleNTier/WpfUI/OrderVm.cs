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
      DoRefresh(() => BusinessLibrary.Order.GetOrder(441));
    }

    protected override void OnError(Exception error)
    {
      Bxf.Shell.Instance.ShowError(error.Message, "Error");
    }

    public void Testing()
    {
      var tmp = new LineItemVm { Model = Model.LineItems };
    }
  }

  public class LineItemVm : ViewModel<BusinessLibrary.LineItems>
  {

  }
}
