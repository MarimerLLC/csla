using Csla;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : ReadOnlyListBase<ResourceList, ResourceInfo>
  {
    public void RemoveChild(int resourceId)
    {
      var iro = IsReadOnly;
      IsReadOnly = false;
      try
      {
        var item = this.Where(r => r.Id == resourceId).FirstOrDefault();
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

    public static ResourceList GetEmptyList()
    {
      return DataPortal.Create<ResourceList>();
    }

    public static async Task<ResourceList> GetResourceListAsync()
    {
      return await DataPortal.FetchAsync<ResourceList>();
    }

    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>();
    }

    [Create]
    [RunLocal]
    private void Create()
    { }

    [Fetch]
    private void DataPortal_Fetch()
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
        List<ProjectTracker.Dal.ResourceDto> list = null;
        list = dal.Fetch();
        foreach (var item in list)
          Add(DataPortal.FetchChild<ResourceInfo>(item));
      }
      IsReadOnly = true;
      RaiseListChangedEvents = rlce;
    }
  }
}