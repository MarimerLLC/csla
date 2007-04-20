using System;
using System.Collections.Generic;
using ProjectTracker.Library;

public class PTService : IPTService
{
  #region IPTService Members

  public ProjectData[] GetProjectList()
  {
    // anonymous access allowed
    Security.UseAnonymous();

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
