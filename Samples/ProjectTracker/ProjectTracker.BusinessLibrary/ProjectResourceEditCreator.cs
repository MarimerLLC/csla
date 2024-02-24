using System;
using Csla;

namespace ProjectTracker.Library
{
  /// <summary>
  /// Command object that creates a project-resource link.
  /// </summary>
  [Serializable]
  public class ProjectResourceEditCreator : ReadOnlyBase<ProjectResourceEditCreator>
  {
    public static readonly PropertyInfo<ProjectResourceEdit> ProjectResourceProperty = RegisterProperty<ProjectResourceEdit>(c => c.ProjectResource);
    public ProjectResourceEdit ProjectResource
    {
      get { return GetProperty(ProjectResourceProperty); }
      private set { LoadProperty(ProjectResourceProperty, value); }
    }

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
