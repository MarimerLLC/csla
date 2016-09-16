using System;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceGetter : ReadOnlyBase<ResourceGetter>
  {
    public static PropertyInfo<ResourceEdit> ResourceProperty = RegisterProperty<ResourceEdit>(c => c.Resource);
    public ResourceEdit Resource
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
      DataPortal.BeginFetch<ResourceGetter>(new Criteria { ResourceId = -1, GetRoles = !RoleList.IsCached }, (o, e) =>
      {
        if (e.Error == null)
        {
          if (!RoleList.IsCached)
            RoleList.SetCache(e.Object.RoleList);
        }
        callback(o, e);
      });
    }

    public static void GetExistingResource(int resourceId, EventHandler<DataPortalResult<ResourceGetter>> callback)
    {
      DataPortal.BeginFetch<ResourceGetter>(new Criteria { ResourceId = resourceId, GetRoles = !RoleList.IsCached }, (o, e) =>
      {
        if (e.Error != null)
          throw e.Error;
        if (!RoleList.IsCached)
          RoleList.SetCache(e.Object.RoleList);
        callback(o, e);
      });
    }

#if FULL_DOTNET
    private void DataPortal_Fetch(Criteria criteria)
    {
      if (criteria.ResourceId == -1)
        Resource = ResourceEdit.NewResourceEdit();
      else
        Resource = ResourceEdit.GetResourceEdit(criteria.ResourceId);
      if (criteria.GetRoles)
        RoleList = RoleList.GetCachedList();
    }
#endif

    [Serializable]
    public class Criteria : CriteriaBase<Criteria>
    {
      public static readonly PropertyInfo<int> ResourceIdProperty = RegisterProperty<int>(c => c.ResourceId);
      public int ResourceId
      {
        get { return ReadProperty(ResourceIdProperty); }
        set { LoadProperty(ResourceIdProperty, value); }
      }

      public static readonly PropertyInfo<bool> GetRolesProperty = RegisterProperty<bool>(c => c.GetRoles);
      public bool GetRoles
      {
        get { return ReadProperty(GetRolesProperty); }
        set { LoadProperty(GetRolesProperty, value); }
      }
    }
  }
}
