using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceAssignmentEditCreator : ReadOnlyBase<ResourceAssignmentEditCreator>
  {
    public static PropertyInfo<ResourceAssignmentEdit> ResultProperty = RegisterProperty<ResourceAssignmentEdit>(c => c.Result);
    public ResourceAssignmentEdit Result
    {
      get { return GetProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    public static void GetResourceAssignmentEditCreator(int projectId, EventHandler<DataPortalResult<ResourceAssignmentEditCreator>> callback)
    {
      DataPortal.BeginFetch<ResourceAssignmentEditCreator>(projectId, callback);
    }

#if !NETFX_CORE
    public static ResourceAssignmentEditCreator GetResourceAssignmentEditCreator(int projectId)
    {
      return DataPortal.Fetch<ResourceAssignmentEditCreator>(projectId);
    }

    private void DataPortal_Fetch(int projectId)
    {
      Result = DataPortal.CreateChild<ResourceAssignmentEdit>(projectId);
    }
#endif
  }
}
