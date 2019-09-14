using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
    public LineItems()
    {
      AllowNew = true;
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
