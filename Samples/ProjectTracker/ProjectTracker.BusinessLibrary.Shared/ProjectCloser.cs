using System;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectCloser : CommandBase<ProjectCloser>
  {
    public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(c => c.ProjectId);
    public int ProjectId
    {
      get { return ReadProperty(ProjectIdProperty); }
      private set { LoadProperty(ProjectIdProperty, value); }
    }

    public static readonly PropertyInfo<bool> ClosedProperty = RegisterProperty<bool>(c => c.Closed);
    public bool Closed
    {
      get { return ReadProperty(ClosedProperty); }
      private set { LoadProperty(ClosedProperty, value); }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(
        typeof(ProjectCloser), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, "ProjectManager"));
    }

    public static void CloseProject(int id, EventHandler<DataPortalResult<ProjectCloser>> callback)
    {
      ProjectCloser cmd = new ProjectCloser { ProjectId = id };
      DataPortal.BeginExecute<ProjectCloser>(cmd, callback);
    }

#if FULL_DOTNET
    public static ProjectCloser CloseProject(int id)
    {
      ProjectCloser cmd = new ProjectCloser { ProjectId = id };
      cmd = DataPortal.Execute<ProjectCloser>(cmd);
      return cmd;
    }

    protected override void DataPortal_Execute()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
        var data = dal.Fetch(ProjectId);
        if (data.Ended.HasValue)
          throw new InvalidOperationException("Project already closed");
        data.Ended = DateTime.Today;
        dal.Update(data);
      }
      Closed = true;
    }
#endif
  }
}