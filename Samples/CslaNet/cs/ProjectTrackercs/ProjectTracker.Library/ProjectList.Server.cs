using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  public partial class ProjectList
  {
    #region  Factory Methods

    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>();
    }

    public static ProjectList GetProjectList(string name)
    {
      return DataPortal.Fetch<ProjectList>(new SingleCriteria<ProjectList, string>(name));
    }

    private ProjectList()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch()
    {
      // fetch with no filter
      Fetch("");
    }

    private void DataPortal_Fetch(Csla.SingleCriteria<ProjectList, string> criteria)
    {
      Fetch(criteria.Value);
    }

    private void Fetch(string nameFilter)
    {
      RaiseListChangedEvents = false;
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        var data = from p in ctx.DataContext.Projects
                   select new ProjectInfo(p.Id, p.Name);
        if (!(string.IsNullOrEmpty(nameFilter)))
        {
          data = from p in ctx.DataContext.Projects
                 where p.Name.Contains(nameFilter)
                 select new ProjectInfo(p.Id, p.Name);
        }
        IsReadOnly = false;
        this.AddRange(data);
        IsReadOnly = true;
      }
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
