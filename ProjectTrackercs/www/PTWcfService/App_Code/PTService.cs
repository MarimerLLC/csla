using System;
using System.Collections.Generic;
using ProjectTracker.Library;
using System.ServiceModel;

public class PTService : IPTService
{
  #region IPTService Members

  public List<ProjectData> GetProjectList(ProjectListRequest request)
  {
    // TODO: comment out the following if using the
    // PTWcfServiceAuth components to require a
    // username/password from the caller
    ProjectTracker.Library.Security.PTPrincipal.Logout();

    try
    {
      ProjectList list = null;
      if (string.IsNullOrEmpty(request.Name))
        list = ProjectList.GetProjectList();
      else
        list = ProjectList.GetProjectList(request.Name);
      List<ProjectData> result = new List<ProjectData>();
      foreach (ProjectInfo item in list)
      {
        ProjectData info = new ProjectData();
        Csla.Data.DataMapper.Map(item, info);
        result.Add(info);
      }
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw new FaultException(ex.BusinessException.Message);
    }
    catch (Exception ex)
    {
      throw new FaultException(ex.Message);
    }
  }

  public ProjectData GetProject(ProjectRequest request)
  {
    try
    {
      var project = Project.GetProject(request.Id);
      var result = new ProjectData();
      Csla.Data.DataMapper.Map(project, result, "Resources");
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw new FaultException(ex.BusinessException.Message);
    }
    catch (Exception ex)
    {
      throw new FaultException(ex.Message);
    }
  }

  public ProjectData AddProject(AddProjectRequest request)
  {
    // TODO: comment out the following if using the
    // PTWcfServiceAuth components to require a
    // username/password from the caller
    //ProjectTracker.Library.Security.PTPrincipal.LoadPrincipal("pm");

    try
    {
      ProjectData obj = request.ProjectData;
      var project = Project.NewProject();
      project.Name = obj.Name;
      project.Started = obj.Started;
      project.Ended = obj.Ended;
      project.Description = obj.Description;
      project = project.Save();

      ProjectData result = new ProjectData();
      Csla.Data.DataMapper.Map(project, result, "Resources");
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw new FaultException(ex.BusinessException.Message);
    }
    catch (Exception ex)
    {
      throw new FaultException(ex.Message);
    }
  }

  public ProjectData UpdateProject(UpdateProjectRequest request)
  {
    // TODO: comment out the following if using the
    // PTWcfServiceAuth components to require a
    // username/password from the caller
    //ProjectTracker.Library.Security.PTPrincipal.LoadPrincipal("pm");

    try
    {
      ProjectData obj = request.ProjectData;
      var project = Project.GetProject(obj.Id);
      project.Name = obj.Name;
      project.Started = obj.Started;
      project.Ended = obj.Ended;
      project.Description = obj.Description;
      project = project.Save();

      ProjectData result = new ProjectData();
      Csla.Data.DataMapper.Map(project, result, "Resources");
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw new FaultException(ex.BusinessException.Message);
    }
    catch (Exception ex)
    {
      throw new FaultException(ex.Message);
    }
  }

  public List<RoleData> GetRoles(RoleRequest request)
  {
    try
    {
      RoleList list = RoleList.GetList();
      List<RoleData> result = new List<RoleData>();
      foreach (var item in list)
      {
        RoleData info = new RoleData();
        Csla.Data.DataMapper.Map(item, info);
        result.Add(info);
      }
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw new FaultException(ex.BusinessException.Message);
    }
    catch (Exception ex)
    {
      throw new FaultException(ex.Message);
    }
  }

  #endregion
}
