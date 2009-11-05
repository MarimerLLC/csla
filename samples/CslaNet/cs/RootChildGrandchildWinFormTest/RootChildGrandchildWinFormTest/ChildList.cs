using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, Child>
  {
    protected override object AddNewCore()
    {
      Child item = new Child();
      Add(item);
      return item;
    }

    public ChildList()
    {
      AllowNew = true;
    }

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
