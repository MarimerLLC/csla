using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class ChildList : BusinessBindingListBase<ChildList, Child>
  {

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("  {0} {1}: {2}\r", this.GetType().Name, "n/a", this.EditLevel);
      foreach (Child item in DeletedList)
        item.DumpEditLevels(sb);
      foreach (Child item in this)
        item.DumpEditLevels(sb);
    }
  }
}
