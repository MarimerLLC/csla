using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
    [FetchChild]
    private void Fetch()
    { }
  }
}
