using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceGetter : ReadOnlyBase<ResourceGetter>
  {
    public static PropertyInfo<Resource> ResourceProperty = RegisterProperty<Resource>(c => c.Resource);
    public Resource Resource
    {
      get { return GetProperty(ResourceProperty); }
      private set { LoadProperty(ResourceProperty, value); }
    }

    public static PropertyInfo<RoleList> RoleListProperty = RegisterProperty<RoleList>(c => c.RoleList);
    public RoleList RoleList
    {
      get { return GetProperty(RoleListProperty); }
      private set { LoadProperty(RoleListProperty, value); }
    }

    public static void CreateNewResource(EventHandler<DataPortalResult<ResourceGetter>> callback)
    {
      DataPortal.BeginCreate<ResourceGetter>(callback);
    }

    public static void GetExistingResource(int resourceId, EventHandler<DataPortalResult<ResourceGetter>> callback)
    {
      DataPortal.BeginFetch<ResourceGetter>(resourceId, callback);
    }
  }
}
