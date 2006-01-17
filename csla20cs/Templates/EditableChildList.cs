using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
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

    private EditableChildList(SqlDataReader dr)
    {
      MarkAsChild();
      Fetch(dr);
    }

    #endregion

    #region Data Access

    private void Fetch(SqlDataReader dr)
    {
      while (dr.Read())
      {
        this.Add(EditableChild.GetEditableChild(dr));
      }
    }


    internal void Update()
    {
      foreach (EditableChild item in DeletedList)
        item.DeleteSelf();
      DeletedList.Clear();

      foreach (EditableChild item in this)
        if (item.IsNew)
          item.Insert();
        else
          item.Update();
    }

    #endregion
  }
}
