using System;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectGetter : ReadOnlyBase<ProjectGetter>
  {
    public static readonly PropertyInfo<ProjectEdit> ProjectProperty = RegisterProperty<ProjectEdit>(c => c.Project);
    public ProjectEdit Project
    {
      get { return GetProperty(ProjectProperty); }
      private set { LoadProperty(ProjectProperty, value); }
    }

    public static readonly PropertyInfo<RoleList> RoleListProperty = RegisterProperty<RoleList>(c => c.RoleList);
    public RoleList RoleList
    {
      get { return GetProperty(RoleListProperty); }
      private set { LoadProperty(RoleListProperty, value); }
    }

#if FULL_DOTNET
    public static void CreateNewProject(EventHandler<DataPortalResult<ProjectGetter>> callback)
    {
      DataPortal.BeginFetch<ProjectGetter>(new Criteria { ProjectId = -1, GetRoles = !RoleList.IsCached }, (o, e) =>
      {
        if (e.Error != null)
          throw e.Error;
        if (!RoleList.IsCached)
          RoleList.SetCache(e.Object.RoleList);
        callback(o, e);
      });
    }

    public static void GetExistingProject(int projectId, EventHandler<DataPortalResult<ProjectGetter>> callback)
    {
      DataPortal.BeginFetch<ProjectGetter>(new Criteria { ProjectId = projectId, GetRoles = !RoleList.IsCached }, (o, e) =>
      {
        if (e.Error != null)
          throw e.Error;
        if (!RoleList.IsCached)
          RoleList.SetCache(e.Object.RoleList);
        callback(o, e);
      });
    }
#endif

    private async Task DataPortal_Fetch(Criteria criteria)
    {
      if (criteria.ProjectId == -1)
        Project = await ProjectEdit.NewProjectAsync();
      else
        Project = await ProjectEdit.GetProjectAsync(criteria.ProjectId);
      if (criteria.GetRoles)
        RoleList = RoleList.GetCachedList();
    }

    [Serializable]
    public class Criteria : CriteriaBase<Criteria>
    {
      public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(c => c.ProjectId);
      public int ProjectId
      {
        get { return ReadProperty(ProjectIdProperty); }
        set { LoadProperty(ProjectIdProperty, value); }
      }

      public static readonly PropertyInfo<bool> GetRolesProperty = RegisterProperty<bool>(c => c.GetRoles);
      public bool GetRoles
      {
        get { return ReadProperty(GetRolesProperty); }
        set { LoadProperty(GetRolesProperty, value); }
      }
    }
  }
}
