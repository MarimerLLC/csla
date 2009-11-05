using Csla;
using System;

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
        ResourceAssignment project = ResourceAssignment.NewResourceAssignment(projectId);
        this.Add(project);
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

  }
}