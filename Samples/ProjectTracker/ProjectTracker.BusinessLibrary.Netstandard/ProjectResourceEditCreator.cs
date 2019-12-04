using System;
using System.Threading.Tasks;
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
    public static async Task<ProjectResourceEditCreator> GetProjectResourceEditCreatorAsync(int resourceId)
    {
      return await DataPortal.FetchAsync<ProjectResourceEditCreator>(resourceId);
    }

    /// <summary>
    /// Gets an existing ProjectResourceEdit object.
    /// </summary>
    public static async Task<ProjectResourceEditCreator> GetProjectResourceEditCreatorAsync(int projectId, int resourceId)
    {
      return await DataPortal.FetchAsync<ProjectResourceEditCreator>(projectId, resourceId);
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
      return DataPortal.Fetch<ProjectResourceEditCreator>(projectId, resourceId);
    }

    [Fetch]
    private void Fetch(int resourceId)
    {
      Result = DataPortal.CreateChild<ProjectResourceEdit>(resourceId);
    }

    [Fetch]
    private void Fetch(int projectId, int resourceId)
    {
      Result = DataPortal.FetchChild<ProjectResourceEdit>(projectId, resourceId);
    }
  }
}
