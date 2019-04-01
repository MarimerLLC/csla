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

    /// <summary>
    /// Creates a new ProjectResourceEdit object.
    /// </summary>
    public static void GetProjectResourceEditCreator(int resourceId, EventHandler<DataPortalResult<ProjectResourceEditCreator>> callback)
    {
      DataPortal.BeginFetch<ProjectResourceEditCreator>(resourceId, callback);
    }

    /// <summary>
    /// Gets an existing ProjectResourceEdit object.
    /// </summary>
    public static void GetProjectResourceEditCreator(int projectId, int resourceId, 
      EventHandler<DataPortalResult<ProjectResourceEditCreator>> callback)
    {
      DataPortal.BeginFetch<ProjectResourceEditCreator>(
        new ProjectResourceCriteria { ProjectId = projectId, ResourceId = resourceId }, callback);
    }

    /// <summary>
    /// Creates a new ProjectResourceEdit object.
    /// </summary>
    public static ProjectResourceEditCreator GetProjectResourceEditCreator(int resourceId)
    {
      return DataPortal.Fetch<ProjectResourceEditCreator>(resourceId);
    }

    /// <summary>
    /// Gets an existing ProjectResourceEdit object.
    /// </summary>
    public static ProjectResourceEditCreator GetProjectResourceEditCreator(int projectId, int resourceId)
    {
      return DataPortal.Fetch<ProjectResourceEditCreator>(new ProjectResourceCriteria { ProjectId = projectId, ResourceId = resourceId });
    }

    private void DataPortal_Fetch(int resourceId)
    {
      Result = DataPortal.CreateChild<ProjectResourceEdit>(resourceId);
    }

    private void DataPortal_Fetch(ProjectResourceCriteria criteria)
    {
      Result = DataPortal.FetchChild<ProjectResourceEdit>(criteria.ProjectId, criteria.ResourceId);
    }

    [Serializable]
    public class ProjectResourceCriteria : CriteriaBase<ProjectResourceCriteria>
    {
      public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(c => c.ProjectId);
      public int ProjectId
      {
        get { return ReadProperty(ProjectIdProperty); }
        set { LoadProperty(ProjectIdProperty, value); }
      }

      public static readonly PropertyInfo<int> ResourceIdProperty = RegisterProperty<int>(c => c.ResourceId);
      public int ResourceId
      {
        get { return ReadProperty(ResourceIdProperty); }
        set { LoadProperty(ResourceIdProperty, value); }
      }
    }
  }
}
