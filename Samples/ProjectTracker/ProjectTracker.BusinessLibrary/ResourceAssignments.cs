using System;
using System.Linq;
using System.Threading.Tasks;
using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceAssignments : BusinessListBase<ResourceAssignments, ResourceAssignmentEdit>
  {
    public async Task<ResourceAssignmentEdit> AssignToAsync(int projectId)
    {
      if (!(Contains(projectId)))
      {
        var creator = ApplicationContext.GetRequiredService<IDataPortal<ResourceAssignmentEditCreator>>();
        var project = await creator.FetchAsync(projectId);
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
    private void Fetch(int resourceId, [Inject] IAssignmentDal dal, [Inject] IChildDataPortal<ResourceAssignmentEdit> portal)
    {
      using (LoadListMode)
      {
        var data = dal.FetchForResource(resourceId);
        foreach (var item in data)
          Add(portal.FetchChild(item));
      }
    }
  }
}