using System;
using System.Linq;
using System.Threading.Tasks;
using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignments : BusinessListBase<ResourceAssignments, ResourceAssignmentEdit>
  {
    public ResourceAssignmentEdit AssignTo(int projectId)
    {
      if (!(Contains(projectId)))
      {
        var project = ResourceAssignmentEditCreator.GetResourceAssignmentEditCreator(projectId).Result;
        this.Add(project);
        return project;
      }
      else
      {
        throw new InvalidOperationException("Resource already assigned to project");
      }
    }

    public async Task<ResourceAssignmentEdit> AssignToAsync(int projectId)
    {
      if (!(Contains(projectId)))
      {
        var project = await ResourceAssignmentEditCreator.GetResourceAssignmentEditCreatorAsync(projectId);
        this.Add(project.Result);
        return project.Result;
      }
      else
      {
        throw new InvalidOperationException("Resource already assigned to project");
      }
    }

    public void Remove(int projectId)
    {
      var item = (from r in this
                  where r.ProjectId == projectId
                  select r).FirstOrDefault();
      if (item != null)
        Remove(item);
    }

    public bool Contains(int projectId)
    {
      var count = (from r in this
                   where r.ProjectId == projectId
                   select r).Count();
      return count > 0;
    }

    public bool ContainsDeleted(int projectId)
    {
      var count = (from r in DeletedList
                   where r.ProjectId == projectId
                   select r).Count();
      return count > 0;
    }

    [FetchChild]
    private void Fetch(int resourceId, [Inject] IAssignmentDal dal)
    {
      using (LoadListMode)
      {
        var data = dal.FetchForResource(resourceId);
        foreach (var item in data)
          Add(DataPortal.FetchChild<ResourceAssignmentEdit>(item));
      }
    }
  }
}