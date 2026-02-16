using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectExistsCommand : CommandBase<ProjectExistsCommand>
  {
    private partial int ProjectId { get; set; }

    public partial bool ProjectExists { get; private set; }

    [Execute]
    private void Execute([Inject] IProjectDal dal)
    {
      ProjectExists = dal.Exists(ProjectId);
    }
  }
}
