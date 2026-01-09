using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectGetter : ReadOnlyBase<ProjectGetter>
  {
    public partial ProjectEdit Project { get; private set; }

    [Fetch]
    private async Task Fetch(int projectId, [Inject] IDataPortal<ProjectEdit> portal)
    {
      if (projectId == -1)
        Project = await portal.CreateAsync();
      else
        Project = await portal.FetchAsync(projectId);
    }
  }
}
