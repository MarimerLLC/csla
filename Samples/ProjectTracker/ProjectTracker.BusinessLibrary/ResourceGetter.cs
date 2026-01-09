using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ResourceGetter : ReadOnlyBase<ResourceGetter>
  {
    public partial ResourceEdit Resource { get; private set; }

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
