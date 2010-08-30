using Csla;
using System;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignments : BusinessListBase<ResourceAssignments, ResourceAssignment>
  {
    #region  Business Methods

    public ResourceAssignment this[Guid projectId]
    {
      get
      {
        foreach (ResourceAssignment res in this)
        {
          if (res.ProjectId.Equals(projectId))
          {
            return res;
          }
        }
        return null;
      }
    }

    public void AssignTo(Guid projectId)
    {
      if (!(Contains(projectId)))
      {
#if SILVERLIGHT
        ResourceAssignment.NewResourceAssignment(projectId, (project) =>
          {
            this.Add(project);
          });
#else
        var project = ResourceAssignment.NewResourceAssignment(projectId);
        this.Add(project);
#endif
      }
      else
      {
        throw new InvalidOperationException("Resource already assigned to project");
      }
    }

    public void Remove(Guid projectId)
    {
      foreach (ResourceAssignment res in this)
      {
        if (res.ProjectId.Equals(projectId))
        {
          Remove(res);
          break;
        }
      }
    }

    public bool Contains(Guid projectId)
    {
      foreach (ResourceAssignment project in this)
        if (project.ProjectId == projectId)
          return true;
      return false;
    }

    public bool ContainsDeleted(Guid projectId)
    {
      foreach (ResourceAssignment project in DeletedList)
        if (project.ProjectId == projectId)
          return true;
      return false;

    }

    #endregion

#if SILVERLIGHT
    internal static void NewResourceAssignments(Action<ResourceAssignments> callback)
    {
      DataPortal.BeginExecute<ResourceAssignmentsCreator>(new ResourceAssignmentsCreator(), (o, e) =>
        {
          callback(e.Object.Result);
        });
    }

    [Serializable]
    private class ResourceAssignmentsCreator : CommandBase<ResourceAssignmentsCreator>
    {
      public static PropertyInfo<ResourceAssignments> ResultProperty = RegisterProperty<ResourceAssignments>(c => c.Result);
      public ResourceAssignments Result
      {
        get { return ReadProperty(ResultProperty); }
        set { LoadProperty(ResultProperty, value); }
      }

#if !SILVERLIGHT
      protected override void DataPortal_Execute()
      {
        Result = ResourceAssignments.NewResourceAssignments();
      }
#endif
    }

#else
    #region  Factory Methods

    internal static ResourceAssignments NewResourceAssignments()
    {
      return DataPortal.CreateChild<ResourceAssignments>();
    }

    internal static ResourceAssignments GetResourceAssignments(ProjectTracker.DalLinq.Assignment[] data)
    {
      return DataPortal.FetchChild<ResourceAssignments>(data);
    }

    private ResourceAssignments()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void Child_Fetch(ProjectTracker.DalLinq.Assignment[] data)
    {
      this.RaiseListChangedEvents = false;
      foreach (var child in data)
        Add(ResourceAssignment.GetResourceAssignment(child));
      this.RaiseListChangedEvents = true;
    }

    #endregion
#endif
  }
}