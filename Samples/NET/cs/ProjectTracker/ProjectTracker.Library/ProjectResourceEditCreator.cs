using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceEditCreator : ReadOnlyBase<ProjectResourceEditCreator>
  {
    public static PropertyInfo<ProjectResourceEdit> ResultProperty = RegisterProperty<ProjectResourceEdit>(c => c.Result);
    public ProjectResourceEdit Result
    {
      get { return GetProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    public static void GetProjectResourceEditCreator(int resourceId, EventHandler<DataPortalResult<ProjectResourceEditCreator>> callback)
    {
      DataPortal.BeginFetch<ProjectResourceEditCreator>(resourceId, callback);
    }

#if !SILVERLIGHT
    public static ProjectResourceEditCreator GetProjectResourceEditCreator(int resourceId)
    {
      return DataPortal.Fetch<ProjectResourceEditCreator>(resourceId);
    }

    private void DataPortal_Fetch(int resourceId)
    {
      Result = DataPortal.CreateChild<ProjectResourceEdit>(resourceId);
    }
#endif
  }
}
