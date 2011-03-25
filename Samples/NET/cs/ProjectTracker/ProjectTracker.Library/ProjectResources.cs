using System;
using System.Linq;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResources : BusinessListBase<ProjectResources, ProjectResourceEdit>
  {
    public void Assign(int resourceId)
    {
      if (!(Contains(resourceId)))
      {
        ProjectResourceEdit resource = DataPortal.CreateChild<ProjectResourceEdit>(resourceId);
        this.Add(resource);
      }
      else
      {
        throw new InvalidOperationException("Resource already assigned to project");
      }
    }

    public void Remove(int resourceId)
    {
      var item = (from r in this
                    where r.ResourceId == resourceId
                    select r).FirstOrDefault();
      if (item != null)
        Remove(item);
    }

    public bool Contains(int resourceId)
    {
      var item = (from r in this
                  where r.ResourceId == resourceId
                  select r).Count();
      return item > 0;
    }

    public bool ContainsDeleted(int resourceId)
    {
      var item = (from r in DeletedList
                  where r.ResourceId == resourceId
                  select r).Count();
      return item > 0;
    }
  }
}