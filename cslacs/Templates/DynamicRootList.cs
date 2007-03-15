using System;
using System.Data.SqlClient;
using Csla;

[Serializable()]
public class DynamicRootList : 
  EditableRootListBase<DynamicRoot>
{

  #region Business Methods

  protected override object AddNewCore()
  {
    DynamicRoot item = DynamicRoot.NewDynamicRoot();
    Add(item);
    return item;
  }

  #endregion

  #region  Authorization Rules

  public static bool CanGetObject()
  {
    // TODO: customize to check user role
    return ApplicationContext.User.IsInRole("");
  }

  public static bool CanEditObject()
  {
    // TODO: customize to check user role
    return ApplicationContext.User.IsInRole("");
  }

#endregion

#region  Factory Methods 

  public static DynamicRootList NewDynamicRootList()
  {
    return new DynamicRootList();
  }

  public static DynamicRootList GetDynamicRootList()
  {
    return DataPortal.Fetch<DynamicRootList>();
  }

  private DynamicRootList()
  {
    // require use of factory methods
    AllowNew = true;
  }

#endregion

#region  Data Access 

  private void DataPortal_Fetch()
  {

    // TODO: load values
    RaiseListChangedEvents = false;
    using (SqlDataReader dr = null)
    {
      while (dr.Read())
      {
        Add(DynamicRoot.GetDynamicRoot(dr));
      }
    }
    RaiseListChangedEvents = true;

  }

#endregion

}
