using System;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceGetter : ReadOnlyBase<ResourceGetter>
  {
    public static readonly PropertyInfo<ResourceEdit> ResourceProperty = RegisterProperty<ResourceEdit>(c => c.Resource);
    public ResourceEdit Resource
    {
      get { return GetProperty(ResourceProperty); }
      private set { LoadProperty(ResourceProperty, value); }
    }

    public static readonly PropertyInfo<RoleList> RoleListProperty = RegisterProperty<RoleList>(c => c.RoleList);
    public RoleList RoleList
    {
      get { return GetProperty(RoleListProperty); }
      private set { LoadProperty(RoleListProperty, value); }
    }

    public static async Task<ResourceEdit> CreateNewResource()
    {
      return await GetExistingResource(-1);
    }

    public static async Task<ResourceEdit> GetExistingResource(int resourceId)
    {
      var result = await DataPortal.FetchAsync<ResourceGetter>(resourceId, !RoleList.IsCached);
      if (!RoleList.IsCached)
        RoleList.SetCache(result.RoleList);
      return result.Resource;
    }

    [Fetch]
    private void Fetch(int resourceId, bool getRoles)
    {
      if (resourceId == -1)
        Resource = ResourceEdit.NewResourceEdit();
      else
        Resource = ResourceEdit.GetResourceEdit(resourceId);
      if (getRoles)
        RoleList = RoleList.GetCachedList();
    }
  }
}
