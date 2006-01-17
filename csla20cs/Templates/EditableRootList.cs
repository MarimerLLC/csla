using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class EditableRootList : 
    BusinessListBase<EditableRootList, EditableChild>
  {
    #region Authorization Rules

    public static bool CanAddObject()
    {
      return ApplicationContext.User.IsInRole("");
    }

    public static bool CanGetObject()
    {
      return ApplicationContext.User.IsInRole("");
    }

    public static bool CanEditObject()
    {
      return ApplicationContext.User.IsInRole("");
    }

    public static bool CanDeleteObject()
    {
      return ApplicationContext.User.IsInRole("");
    }

    #endregion

    #region Factory Methods

    public static EditableRootList NewEditableRootList()
    {
      return new EditableRootList();
    }

    public static EditableRootList GetEditableRootList(int id)
    {
      return DataPortal.Fetch<EditableRootList>(new Criteria(id));
    }

    private EditableRootList()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }
      public Criteria(int id)
      { _id = id; }
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      // TODO: load values
      using (SqlDataReader dr = null)
      {
        while (dr.Read())
        {
          this.Add(EditableChild.GetEditableChild(dr));
        }
      }
    }

    protected override void DataPortal_Update()
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
