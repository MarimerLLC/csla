using System;
using System.ComponentModel;
using Csla;
using ProjectTracker.Dal;

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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(
        typeof(ProjectCloser), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, Security.Roles.ProjectManager));
    }

    [Create]
    [RunLocal]
    private void Create(int id)
    {
      ProjectId = id;
    }

    [Execute]
    private void Execute([Inject] IProjectDal dal)
    {
      var data = dal.Fetch(ProjectId);
      if (data.Ended.HasValue)
        throw new InvalidOperationException("Project already closed");
      data.Ended = DateTime.Today;
      dal.Update(data);
      Closed = true;
    }
  }
}