using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  class EditableChildList : 
    BusinessListBase<EditableChildList, EditableChild>
  {
    #region Factory Methods

    internal static EditableChildList NewEditableChildList()
    {
      return new EditableChildList();
    }

    internal static EditableChildList GetEditableChildList(SqlDataReader dr)
    {
      return new EditableChildList(dr);
    }

    private EditableChildList()
    {
      MarkAsChild();
    }

    #endregion

    #region Data Access

    private EditableChildList(SqlDataReader dr)
    {
      MarkAsChild();
      while (dr.Read())
      {
        this.Add(EditableChild.GetEditableChild(dr));
      }
    }

    internal void Update()
    {
      foreach (EditableChild item in DeletedList)
        item.Update();
      DeletedList.Clear();

      foreach (EditableChild item in this)
        item.Update();
    }

    #endregion
  }
}
