using System;
using System.ComponentModel;
using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectCloser : CommandBase<ProjectCloser>
  {
    public partial int ProjectId { get; private set; }

    public partial bool Closed { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
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
      var data = dal.Fetch(ProjectId) ?? throw new DataNotFoundException("Project");
      if (data.Ended.HasValue)
        throw new InvalidOperationException("Project already closed");
      data.Ended = DateTime.Today;
      dal.Update(data);
      Closed = true;
    }
  }
}