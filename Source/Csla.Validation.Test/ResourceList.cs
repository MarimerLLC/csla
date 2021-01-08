using Csla;
using Csla.Data;
using System;
using System.Linq;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : ReadOnlyListBase<ResourceList, ResourceInfo>
  {
    #region  Factory Methods

    public static ResourceList EmptyList()
    {
      return new ResourceList();
    }

    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>();
    }

    #endregion

    #region  Data Access    
    
    protected static void AddObjectAuthorizationRules()
    {
      // add object-level authorization rules here
      Csla.Security.AuthorizationRules.AllowCreate(typeof(ResourceList), "ProjectManager");
      Csla.Security.AuthorizationRules.AllowGet(typeof(ResourceList), "Administrator");
    }

    private void DataPortal_Fetch()
    {
      IsReadOnly = false;

      Add(new ResourceInfo(1, "Lhotka", "Rocky"));

      IsReadOnly = true;
    }

    #endregion

  }
}