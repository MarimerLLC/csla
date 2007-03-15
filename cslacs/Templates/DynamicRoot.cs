using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

[Serializable]
public class DynamicRoot : BusinessBase<DynamicRoot>
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

  public static DynamicRoot NewDynamicRoot()
  {
    return DataPortal.Create<DynamicRoot>();
  }

  internal static DynamicRoot GetDynamicRoot(SqlDataReader dr)
  {
    return new DynamicRoot(dr);
  }

  private DynamicRoot()
  { /* Require use of factory methods */ }

  private DynamicRoot(SqlDataReader dr)
  {
    Fetch(dr);
  }

  #endregion

  #region Data Access

  protected override void DataPortal_Create()
  {
    // TODO: load default values
  }

  private void Fetch(SqlDataReader dr)
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
    // TODO: delete values
  }

  #endregion
}
