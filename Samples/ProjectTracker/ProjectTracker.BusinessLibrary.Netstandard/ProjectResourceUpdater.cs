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
    public ProjectResourceEdit ProjectResource
    {
      get { return ReadProperty(ProjectResourceProperty); }
      private set { LoadProperty(ProjectResourceProperty, value); }
    }

    public static async Task<ProjectResourceEdit> UpdateAsync(int projectId, ProjectResourceEdit projectResource)
    {
      var cmd = await DataPortal.CreateAsync<ProjectResourceUpdater>(projectId, projectResource);
      cmd = await DataPortal.ExecuteAsync<ProjectResourceUpdater>(cmd);
      return cmd.ProjectResource;
    }

    [Create]
    [RunLocal]
    private void Create(int projectId, ProjectResourceEdit projectResource)
    {
      ProjectId = projectId;
      ProjectResource = projectResource;
    }

    [Execute]
    private void Execute()
    {
      DataPortal.UpdateChild(ProjectResource, ProjectId);
    }
  }
}
