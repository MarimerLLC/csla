using System;
using Csla;

namespace BusinessLibrary
{
  public class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
    [FetchChild]
    private void Fetch()
    { }
  }
}
