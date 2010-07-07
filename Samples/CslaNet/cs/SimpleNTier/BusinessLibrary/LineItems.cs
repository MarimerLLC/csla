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
    // Force creation by factory methods 
    private LineItems()
    {
    }

    internal static LineItems NewList()
    {
      return DataPortal.CreateChild<LineItems>();
    }
  }
}
