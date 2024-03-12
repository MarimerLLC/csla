using System;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectGetter : ReadOnlyBase<ProjectGetter>
  {
    public static readonly PropertyInfo<ProjectEdit> ProjectProperty = RegisterProperty<ProjectEdit>(nameof(Project));
    public ProjectEdit Project
    {
      get { return GetProperty(ProjectProperty); }
      private set { LoadProperty(ProjectProperty, value); }
    }

    [Fetch]
    private async Task Fetch(int projectId, [Inject] IDataPortal<ProjectEdit> portal)
    {
      if (projectId == -1)
        Project = await portal.CreateAsync();
      else
        Project = await portal.FetchAsync(projectId);
    }
  }
}
