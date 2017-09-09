using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace MultipleBindingSources
{
  [Serializable]
  public class ChildList :
    Csla.BusinessBindingListBase<ChildList, Child>
  {
    #region Factory Methods

    internal static ChildList NewChildList()
    {
      var list =  DataPortal.CreateChild<ChildList>();

      list.Add(Child.NewEditableChild());
      list.Add(Child.NewEditableChild());

      return list;
    }

    internal static ChildList GetChildList(
      object childData)
    {
      return DataPortal.FetchChild<ChildList>(childData);
    }

    public ChildList()
    {
      AllowNew = true;
      MarkAsChild();
    }

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("  {0} {1}: {2}\r", this.GetType().Name, "n/a", this.EditLevel);
      foreach (Child item in DeletedList)
        item.DumpEditLevels(sb);
      foreach (Child item in this)
        item.DumpEditLevels(sb);
    }
    #endregion

    #region Data Access

    private void Create_Child(object riteria)
    {
      base.Child_Create();

    }
    #endregion
  }
}
