using System;
using System.Linq;
using Csla;
using Csla.Data;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
  {
#if !SILVERLIGHT
    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>();
    }

    public static ProjectList GetProjectList(string name)
    {
      return DataPortal.Fetch<ProjectList>(new SingleCriteria<ProjectList, string>(name));
    }
#endif

    public static void NewProjectList(EventHandler<DataPortalResult<ProjectList>> callback)
    {
      DataPortal.BeginCreate<ProjectList>(callback);
    }

    public static void GetProjectList(string name, EventHandler<DataPortalResult<ProjectList>> callback)
    {
      DataPortal.BeginCreate<ProjectList>(name, callback);
    }
  }
}