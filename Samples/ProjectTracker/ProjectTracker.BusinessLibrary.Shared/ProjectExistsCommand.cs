using System;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectExistsCommand : CommandBase<ProjectExistsCommand>
  {
    public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(c => c.ProjectId);
    private int ProjectId
    {
      get { return ReadProperty(ProjectIdProperty); }
      set { LoadProperty(ProjectIdProperty, value); }
    }

    public static readonly PropertyInfo<bool> ProjectExistsProperty = RegisterProperty<bool>(c => c.ProjectExists);
    public bool ProjectExists
    {
      get { return ReadProperty(ProjectExistsProperty); }
      private set { LoadProperty(ProjectExistsProperty, value); }
    }

    public ProjectExistsCommand()
    { }

#pragma warning disable CSLA0004
    public ProjectExistsCommand(int id)
    {
      ProjectId = id;
    }

#if FULL_DOTNET 
    protected override void DataPortal_Execute()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
        ProjectExists = dal.Exists(ProjectId);
      }
    }
#endif
  }
}
