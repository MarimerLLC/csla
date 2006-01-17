using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Templates
{
  class EditableRoot : BusinessBase<EditableRoot>
  {
    #region Business Methods

    private int _id;

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

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //ValidationRules.AddRule(null, "");
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowWrite("", "");
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

    public static EditableRoot NewEditableRoot()
    {
      return DataPortal.Create<EditableRoot>();
    }

    public static EditableRoot GetEditableRoot(int id)
    {
      return DataPortal.Create<EditableRoot>(new Criteria(id));
    }

    public static void DeleteEditableRoot(int id)
    {
      DataPortal.Delete(new Criteria(id));
    }

    private EditableRoot()
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
    }

    protected override void DataPortal_Insert()
    {
      // TODO: insert values
    }

    protected override void DataPortal_Update()
    {
      // TODO: update values
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
