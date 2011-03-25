using Csla;
using Csla.Data;
using System;
using System.Linq;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : ReadOnlyListBase<ResourceList, ResourceInfo>
  {
    public static ResourceList EmptyList()
    {
      return new ResourceList();
    }

    public static void GetResourceList(EventHandler<DataPortalResult<ResourceList>> callback)
    {
      DataPortal.BeginFetch<ResourceList>(callback);
    }

#if !SILVERLIGHT
    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>();
    }

    private void DataPortal_Fetch()
    {

    }
#endif
  }
}