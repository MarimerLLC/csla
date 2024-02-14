using System;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceEditCreator : ReadOnlyBase<ProjectResourceEditCreator>
  {
    public static readonly PropertyInfo<ProjectResourceEdit> ResultProperty = RegisterProperty<ProjectResourceEdit>(c => c.Result);
    public ProjectResourceEdit Result
    {
      get { return GetProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    [Fetch]
    private void Fetch(int resourceId, [Inject] IChildDataPortal<ProjectResourceEdit> portal)
    {
      Result = portal.CreateChild(resourceId);
    }

    [Fetch]
    private void Fetch(int projectId, int resourceId, [Inject] IChildDataPortal<ProjectResourceEdit> portal)
    {
      Result = portal.FetchChild(projectId, resourceId);
    }
  }
}
