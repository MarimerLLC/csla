using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class SwitchableObject : BusinessBase<SwitchableObject>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods
    private int _id;

    public int id
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get 
      {
        CanReadProperty(true);
        return _id; 
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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

    public static SwitchableObject NewSwitchableRoot()
    {
      return DataPortal.Create<SwitchableObject>(
        new RootCriteria());
    }

    internal static SwitchableObject NewSwitchableChild()
    {
      return DataPortal.Create<SwitchableObject>(
        new ChildCriteria());
    }

    public static SwitchableObject GetSwitchableRoot(int id)
    {
      return DataPortal.Fetch<SwitchableObject>(
        new RootCriteria(id));
    }

    internal static SwitchableObject GetSwitchableChild(
      SqlDataReader dr)
    {
      return new SwitchableObject(dr);
    }

    public static void DeleteSwitchableObject(int id)
    {
      DataPortal.Delete(new RootCriteria(id));
    }

    private SwitchableObject()
    { /* Require use of factory methods */ }

    private SwitchableObject(SqlDataReader dr)
    {
      Fetch(dr);
    }

    #endregion

    #region Data Access

    [Serializable()]
    private class RootCriteria
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }
      public RootCriteria(int id)
      { _id = id; }
      public RootCriteria()
      { }
    }

    [Serializable()]
    private class ChildCriteria
    { }

    private void DataPortal_Create(RootCriteria criteria)
    {
      DoCreate();
    }

    private void DataPortal_Create(ChildCriteria criteria)
    {
      MarkAsChild();
      DoCreate();
    }

    private void DoCreate()
    {
      // TODO: load default values
    }

    private void DataPortal_Fetch(RootCriteria criteria)
    {
      // TODO: create data reader to load values
      using (SqlDataReader dr = null)
      {
        DoFetch(dr);
      }
    }

    private void Fetch(SqlDataReader dr)
    {
      MarkAsChild();
      DoFetch(dr);
    }

    private void DoFetch(SqlDataReader dr)
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
      DataPortal_Delete(new RootCriteria(_id));
    }

    private void DataPortal_Delete(RootCriteria criteria)
    {
      // TODO: delete values
    }

    internal void Insert(object parent)
    {
      // TODO: insert values
      MarkOld();
    }

    internal void Update(object parent)
    {
      // TODO: update values
      MarkOld();
    }

    internal void DeleteSelf()
    {
      // TODO: delete values
      MarkNew();
    }


    #endregion
  }
}
