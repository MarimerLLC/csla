using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class EditableRootParent : BusinessBase<EditableRootParent>
  {
    #region Business Methods

    private int _id;
    private EditableChildList _children =
      EditableChildList.NewEditableChildList();

    public int id
    {
      get
      {
        CanReadProperty(true);
        return _id;
      }
      set
      {
        CanWriteProperty(true);
        if (_id != value)
        {
          _id = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    public EditableChildList Children
    {
      get { return _children; }
    }

    public override bool IsValid
    {
      get
      {
        return base.IsValid && _children.IsValid;
      }
    }

    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _children.IsDirty;
      }
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
    }

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

    public static EditableRootParent NewEditableRootParent()
    {
      return DataPortal.Create<EditableRootParent>();
    }

    public static EditableRootParent GetEditableRootParent(int id)
    {
      return DataPortal.Create<EditableRootParent>(new Criteria(id));
    }

    public static void DeleteEditableRootParent(int id)
    {
      DataPortal.Delete(new Criteria(id));
    }

    private EditableRootParent()
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

    private void DataPortal_Create(Criteria criteria)
    {
      // TODO: load default values
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      // TODO: load values
      using (SqlDataReader dr = null)
      {
        _children =
          EditableChildList.GetEditableChildList(dr);
      }
    }

    protected override void DataPortal_Insert()
    {
      // TODO: insert values
      _children.Update(this);
    }

    protected override void DataPortal_Update()
    {
      // TODO: update values
      _children.Update(this);
    }

    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new Criteria(_id));
    }

    private void DataPortal_Delete(Criteria criteria)
    {
      // TODO: delete values
    }

    #endregion

  }
}
