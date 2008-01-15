using Csla;
using Csla.Data;
using System;
using System.Linq;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : ReadOnlyListBase<ResourceList, ResourceInfo>
  {
    #region  Factory Methods

    public static ResourceList EmptyList()
    {
      return new ResourceList();
    }

    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>();
    }

    private ResourceList()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(Database.PTrackerConnection))
      {
        var data = from r in ctx.DataContext.Resources
                   select r;
        IsReadOnly = false;
        foreach (var resource in data)
          this.Add(new ResourceInfo(resource));
        IsReadOnly = true;
      }
      RaiseListChangedEvents = true;
    }

    #endregion

  }
}