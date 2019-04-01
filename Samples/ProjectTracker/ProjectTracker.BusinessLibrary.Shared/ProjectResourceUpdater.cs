using System;
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

#if FULL_DOTNET
    public static ProjectResourceEdit Update(int projectId, ProjectResourceEdit projectResource)
    {
      var cmd = new ProjectResourceUpdater { ProjectId = projectId, ProjectResource = projectResource };
      cmd = DataPortal.Execute<ProjectResourceUpdater>(cmd);
      return cmd.ProjectResource;
    }
#endif

    protected override void DataPortal_Execute()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        DataPortal.UpdateChild(ProjectResource, ProjectId);
      }
    }
  }
}
