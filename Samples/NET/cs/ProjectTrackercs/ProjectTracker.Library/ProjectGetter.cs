using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectGetter : ReadOnlyBase<ProjectGetter>
  {
    public static PropertyInfo<Project> ProjectProperty = RegisterProperty<Project>(c => c.Project);
    public Project Project
    {
      get { return GetProperty(ProjectProperty); }
      private set { LoadProperty(ProjectProperty, value); }
    }

    public static PropertyInfo<RoleList> RoleListProperty = RegisterProperty<RoleList>(c => c.RoleList);
    public RoleList RoleList
    {
      get { return GetProperty(RoleListProperty); }
      private set { LoadProperty(RoleListProperty, value); }
    }

    public static void CreateNewProject(EventHandler<DataPortalResult<ProjectGetter>> callback)
    {
      DataPortal.BeginCreate<ProjectGetter>(callback);
    }

    public static void GetExistingProject(Guid projectId, EventHandler<DataPortalResult<ProjectGetter>> callback)
    {
      DataPortal.BeginFetch<ProjectGetter>(projectId, callback);
    }
  }
}
