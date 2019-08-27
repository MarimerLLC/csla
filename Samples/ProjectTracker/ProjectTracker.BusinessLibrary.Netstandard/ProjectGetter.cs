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

    public static readonly PropertyInfo<RoleList> RoleListProperty = RegisterProperty<RoleList>(nameof(RoleList));
    public RoleList RoleList
    {
      get { return GetProperty(RoleListProperty); }
      private set { LoadProperty(RoleListProperty, value); }
    }

    public static async Task<ProjectEdit> CreateNewProject()
    {
      return await GetExistingProject(-1);
    }

    public static async Task<ProjectEdit> GetExistingProject(int projectId)
    {
      var result = await DataPortal.FetchAsync<ProjectGetter>(projectId, !RoleList.IsCached);
      if (!RoleList.IsCached)
        RoleList.SetCache(result.RoleList);
      return result.Project;
    }

    [Fetch]
    private async Task Fetch(int projectId, bool getRoles)
    {
      if (projectId == -1)
        Project = await ProjectEdit.NewProjectAsync();
      else
        Project = await ProjectEdit.GetProjectAsync(projectId);
      if (getRoles)
        RoleList = RoleList.GetCachedList();
    }
  }
}
