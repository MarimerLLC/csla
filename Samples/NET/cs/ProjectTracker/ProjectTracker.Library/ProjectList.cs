using System;
using System.Linq;
using Csla;
using Csla.Data;
using Csla.Serialization;
using System.Collections.Generic;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
  {
    public static void GetProjectList(EventHandler<DataPortalResult<ProjectList>> callback)
    {
      DataPortal.BeginFetch<ProjectList>(callback);
    }

    public static void GetProjectList(string name, EventHandler<DataPortalResult<ProjectList>> callback)
    {
      DataPortal.BeginCreate<ProjectList>(name, callback);
    }
#if !SILVERLIGHT
    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>();
    }

    public static ProjectList GetProjectList(string name)
    {
      return DataPortal.Fetch<ProjectList>(name);
    }

    private void DataPortal_Fetch()
    {
      DataPortal_Fetch(null);
    }

    private void DataPortal_Fetch(string name)
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
        List<ProjectTracker.Dal.ProjectDto> list = null;
        if (name == null)
          list = dal.Fetch();
        else
          list = dal.Fetch(name);
        foreach (var item in list)
          Add(DataPortal.FetchChild<ProjectInfo>(item));
      }
      IsReadOnly = true;
      RaiseListChangedEvents = rlce;
    }
#endif
  }
}