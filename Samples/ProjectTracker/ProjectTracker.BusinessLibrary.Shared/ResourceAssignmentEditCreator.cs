using System;
using Csla;

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

#if FULL_DOTNET || NETSTANDARD2_0
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
