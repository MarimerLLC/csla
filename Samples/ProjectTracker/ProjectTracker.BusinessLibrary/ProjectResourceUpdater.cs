using Csla;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectResourceUpdater : CommandBase<ProjectResourceUpdater>
  {
    public partial int ProjectId { get; private set; }

    public partial ProjectResourceEdit ProjectResource { get; private set; }

    [Create]
    [RunLocal]
    private void Create(int projectId, ProjectResourceEdit projectResource)
    {
      ProjectId = projectId;
      ProjectResource = projectResource;
    }

    [Execute]
    private void Execute([Inject] IChildDataPortal<ProjectResourceEdit> portal)
    {
      portal.UpdateChild(ProjectResource, ProjectId);
    }
  }
}
