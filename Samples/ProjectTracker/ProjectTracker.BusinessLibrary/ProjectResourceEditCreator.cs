using Csla;

namespace ProjectTracker.Library
{
  /// <summary>
  /// Command object that creates a project-resource link.
  /// </summary>
  [CslaImplementProperties]
  public partial class ProjectResourceEditCreator : CommandBase<ProjectResourceEditCreator>
  {
    public partial ProjectResourceEdit ProjectResource { get; private set; }

    [Execute]
    private void Execute(int resourceId, [Inject] IChildDataPortal<ProjectResourceEdit> portal)
    {
      ProjectResource = portal.CreateChild(resourceId);
    }

    [Execute]
    private void Execute(int projectId, int resourceId, [Inject] IChildDataPortal<ProjectResourceEdit> portal)
    {
      ProjectResource = portal.FetchChild(projectId, resourceId);
    }
  }
}
