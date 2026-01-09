using System;
using Csla;

namespace ProjectTracker.Library
{
  /// <summary>
  /// Command object that creates a project-resource link.
  /// </summary>
  [Serializable]
  public class ProjectResourceEditCreator : CommandBase<ProjectResourceEditCreator>
  {
    public static readonly PropertyInfo<ProjectResourceEdit> ProjectResourceProperty = RegisterProperty<ProjectResourceEdit>(c => c.ProjectResource);
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public ProjectResourceEdit ProjectResource
    {
      get { return ReadProperty(ProjectResourceProperty)!; }
      private set { LoadProperty(ProjectResourceProperty, value); }
    }
#pragma warning restore CSLA0007

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
