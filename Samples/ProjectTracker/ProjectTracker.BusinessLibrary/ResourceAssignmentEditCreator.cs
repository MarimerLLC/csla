using System;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceAssignmentEditCreator : ReadOnlyBase<ResourceAssignmentEditCreator>
  {
    public static readonly PropertyInfo<ResourceAssignmentEdit> ResultProperty = RegisterProperty<ResourceAssignmentEdit>(c => c.Result);
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public ResourceAssignmentEdit Result
    {
      get { return GetProperty(ResultProperty)!; }
      private set { LoadProperty(ResultProperty, value); }
    }
#pragma warning restore CSLA0007

    [Fetch]
    private void Fetch(int projectId, [Inject] IChildDataPortal<ResourceAssignmentEdit> portal)
    {
      Result = portal.CreateChild(projectId);
    }
  }
}
