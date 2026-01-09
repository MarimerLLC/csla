using Csla;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ResourceAssignmentEditCreator : ReadOnlyBase<ResourceAssignmentEditCreator>
  {
    public partial ResourceAssignmentEdit Result { get; private set; }

    [Fetch]
    private void Fetch(int projectId, [Inject] IChildDataPortal<ResourceAssignmentEdit> portal)
    {
      Result = portal.CreateChild(projectId);
    }
  }
}
