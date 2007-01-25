using System;
using System.Data.SqlClient;

[Serializable()]
public class DynamicRootList : 
  EditableRootListBase<DynamicRootList, EditableRoot>
{

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
        Add(EditableRoot.GetEditableRoot(dr));
      }
    }
    RaiseListChangedEvents = true;

  }

#endregion

}
