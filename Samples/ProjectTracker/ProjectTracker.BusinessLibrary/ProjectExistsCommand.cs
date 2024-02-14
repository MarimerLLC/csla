using System;
using Csla;
using ProjectTracker.Dal;

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

    [Execute]
    private void Execute([Inject] IProjectDal dal)
    {
      ProjectExists = dal.Exists(ProjectId);
    }
  }
}
