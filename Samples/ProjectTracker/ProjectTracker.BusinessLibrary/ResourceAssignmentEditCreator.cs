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

    [Fetch]
    private void Fetch(int projectId, [Inject] IChildDataPortal<ResourceAssignmentEdit> portal)
    {
      Result = portal.CreateChild(projectId);
    }
  }
}
