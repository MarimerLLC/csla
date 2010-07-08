using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
    // Force creation by factory methods 
    private LineItems()
    {
    }

    protected override object AddNewCore()
    {
      var item = LineItem.NewItem();
      Add(item);
      return item;
    }

    internal static LineItems NewList()
    {
      return DataPortal.CreateChild<LineItems>();
    }
  }
}
