using Csla;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable]
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

    [Create]
    [RunLocal]
    private void Create()
    { }

    [Fetch]
    private void Fetch([Inject] IResourceDal dal, [Inject] IChildDataPortal<ResourceInfo> portal)
    {
      using (LoadListMode)
      {
        List<ProjectTracker.Dal.ResourceDto> list = dal.Fetch();
        foreach (var item in list)
          Add(portal.FetchChild(item));
      }
    }
  }
}