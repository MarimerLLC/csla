using System;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceUpdater : CommandBase<ProjectResourceUpdater>
  {
    public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(c => c.ProjectId);
    public int ProjectId
    {
      get { return ReadProperty(ProjectIdProperty); }
      private set { LoadProperty(ProjectIdProperty, value); }
    }

    public static readonly PropertyInfo<ProjectResourceEdit> ProjectResourceProperty = RegisterProperty<ProjectResourceEdit>(c => c.ProjectResource);
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public ProjectResourceEdit ProjectResource
    {
      get { return ReadProperty(ProjectResourceProperty)!; }
      private set { LoadProperty(ProjectResourceProperty, value); }
    }
#pragma warning restore CSLA0007

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
