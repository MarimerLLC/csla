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
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
    }

    public static bool CanGetObject()
    {
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
    }

    public static bool CanEditObject()
    {
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
    }

    public static bool CanDeleteObject()
    {
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
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
      RaiseListChangedEvents = false;
      // TODO: load values
      using (SqlDataReader dr = null)
      {
        while (dr.Read())
        {
          this.Add(EditableChild.GetEditableChild(dr));
        }
      }
      RaiseListChangedEvents = true;
    }

    protected override void DataPortal_Update()
    {
      RaiseListChangedEvents = false;
      foreach (EditableChild item in DeletedList)
        item.DeleteSelf();
      DeletedList.Clear();

      foreach (EditableChild item in this)
        if (item.IsNew)
          item.Insert(this);
        else
          item.Update(this);
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
