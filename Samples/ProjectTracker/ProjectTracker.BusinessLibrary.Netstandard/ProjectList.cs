using System;
using System.Linq;
using Csla;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
  {
    public void RemoveChild(int projectId)
    {
      var iro = IsReadOnly;
      IsReadOnly = false;
      try
      {
        var item = this.Where(r => r.Id == projectId).FirstOrDefault();
        if (item != null)
        {
          var index = this.IndexOf(item);
          Remove(item);
        }
      }
      finally
      {
        IsReadOnly = iro;
      }
    }

    public async static Task<ProjectList> GetProjectListAsync()
    {
      return await Csla.DataPortal.FetchAsync<ProjectTracker.Library.ProjectList>();
    }

    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>();
    }

    public static ProjectList GetProjectList(string name)
    {
      return DataPortal.Fetch<ProjectList>(name);
    }

    [Fetch]
    private void DataPortal_Fetch()
    {
      DataPortal_Fetch(null);
    }

    [Fetch]
    private void DataPortal_Fetch(string name)
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      using (var ctx = Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<Dal.IProjectDal>();
        List<Dal.ProjectDto> list = null;
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
  }
}