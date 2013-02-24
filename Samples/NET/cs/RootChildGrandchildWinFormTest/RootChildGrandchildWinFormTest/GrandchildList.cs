using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class GrandchildList : BusinessBindingListBase<GrandchildList, Grandchild>
  {

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("      {0} {1}: {2}\r", this.GetType().Name, "n/a", this.EditLevel);
      foreach (Grandchild item in DeletedList)
        item.DumpEditLevels(sb);
      foreach (Grandchild item in this)
        item.DumpEditLevels(sb);
    }
  }
}
