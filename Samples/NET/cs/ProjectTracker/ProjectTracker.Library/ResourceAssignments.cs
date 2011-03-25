using System;
using System.Linq;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignments : BusinessListBase<ResourceAssignments, ResourceAssignmentEdit>
  {
    public void AssignTo(Guid projectId)
    {
      if (!(Contains(projectId)))
      {
        var project = DataPortal.CreateChild<ResourceAssignmentEdit>(projectId);
        this.Add(project);
      }
      else
      {
        throw new InvalidOperationException("Resource already assigned to project");
      }
    }

    public void Remove(Guid projectId)
    {
      var item = (from r in this
                    where r.ProjectId.Equals(projectId)
                    select r).FirstOrDefault();
      if (item != null)
        Remove(item);
    }

    public bool Contains(Guid projectId)
    {
      var count = (from r in this
                    where r.ProjectId.Equals(projectId)
                    select r).Count();
      return count > 0;
    }

    public bool ContainsDeleted(Guid projectId)
    {
      var count = (from r in DeletedList
                    where r.ProjectId.Equals(projectId)
                    select r).Count();
      return count > 0;
    }
  }
}