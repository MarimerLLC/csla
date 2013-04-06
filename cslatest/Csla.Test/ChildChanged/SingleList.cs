using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class SingleList : BusinessListBase<SingleList, SingleRoot>
  {
    public SingleList()
    {
    }

    public SingleList(bool child)
      : this()
    {
      if (child)
        MarkAsChild();
    }
  }
}
