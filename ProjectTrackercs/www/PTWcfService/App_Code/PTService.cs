using System;
using System.Collections.Generic;
using ProjectTracker.Library;
using System.ServiceModel;

public class PTService : IPTService
{
  #region IPTService Members

  public ProjectData[] GetProjectList()
  {
    // TODO: comment out the following if using the
    // PTWcfServiceAuth components to require a
    // username/password from the caller
    ProjectTracker.Library.Security.PTPrincipal.Logout();

    try
    {
      ProjectList list = ProjectList.GetProjectList();
      List<ProjectData> result = new List<ProjectData>();
      foreach (ProjectInfo item in list)
      {
        ProjectData info = new ProjectData();
        Csla.Data.DataMapper.Map(item, info);
        result.Add(info);
      }
      return result.ToArray();
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
    // TODO: comment out the following if using the
    // PTWcfServiceAuth components to require a
    // username/password from the caller
    ProjectTracker.Library.Security.PTPrincipal.Logout();

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

  #endregion
}
