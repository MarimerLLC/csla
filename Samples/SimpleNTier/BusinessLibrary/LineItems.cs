using System;
using Csla;

namespace BusinessLibrary
{
  public partial class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
    [FetchChild]
    private void Fetch()
    { }
  }
}
