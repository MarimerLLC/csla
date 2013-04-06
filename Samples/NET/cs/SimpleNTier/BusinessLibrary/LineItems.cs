using System;
using Csla;
using Csla.Serialization;

namespace BusinessLibrary
{
  [Serializable]
  public class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
#if !SILVERLIGHT
    // Force creation by factory methods 
    private LineItems()
    { }
#endif
  }
}
