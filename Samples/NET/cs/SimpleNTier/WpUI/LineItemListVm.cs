using System;
using Csla.Xaml;

namespace WpUI
{
  public class LineItemListVm : ViewModel<BusinessLibrary.LineItems>
  {
    public LineItemListVm(BusinessLibrary.Order order)
    {
      Header = "line items";
      Model = order.LineItems;
    }

    public override void AddNew(object sender, ExecuteEventArgs e)
    {
      Model.AddNew();
    }
  }
}
