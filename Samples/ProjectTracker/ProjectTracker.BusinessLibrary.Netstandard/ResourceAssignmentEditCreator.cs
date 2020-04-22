using System;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceAssignmentEditCreator : ReadOnlyBase<ResourceAssignmentEditCreator>
  {
    public static readonly PropertyInfo<ResourceAssignmentEdit> ResultProperty = RegisterProperty<ResourceAssignmentEdit>(c => c.Result);
    public ResourceAssignmentEdit Result
    {
      get { return GetProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    public static async Task<ResourceAssignmentEditCreator> GetResourceAssignmentEditCreatorAsync(int projectId)
    {
      return await DataPortal.FetchAsync<ResourceAssignmentEditCreator>(projectId);
    }

    public static ResourceAssignmentEditCreator GetResourceAssignmentEditCreator(int projectId)
    {
      return DataPortal.Fetch<ResourceAssignmentEditCreator>(projectId);
    }

    [Fetch]
    private void Fetch(int projectId)
    {
      Result = DataPortal.CreateChild<ResourceAssignmentEdit>(projectId);
    }
  }
}
