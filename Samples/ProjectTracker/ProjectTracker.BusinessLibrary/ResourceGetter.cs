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

    [Fetch]
    private async Task Fetch(int resourceId, [Inject] IDataPortal<ResourceEdit> portal)
    {
      if (resourceId == -1)
        Resource = await portal.CreateAsync();
      else
        Resource = await portal.FetchAsync(resourceId);
    }
  }
}
