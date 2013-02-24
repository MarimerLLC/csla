using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.DiffGram;

namespace Test.Library
{
  [Serializable]
  public class LineItems : DiffListBase<LineItems, LineItemEdit>
  {
    private void Child_Fetch(int id)
    {
      for (int line = 0; line < 10; line++)
        Add(DataPortal.FetchChild<LineItemEdit>(id, line));
    }
  }
}
