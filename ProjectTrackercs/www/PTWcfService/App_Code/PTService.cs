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
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  #endregion
}
