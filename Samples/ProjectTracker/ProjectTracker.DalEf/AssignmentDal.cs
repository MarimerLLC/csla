using System.Collections.Generic;
using System.Linq;
using ProjectTracker.Dal;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class AssignmentDal : IAssignmentDal
  {
    private readonly PTrackerEntities objectContext;

    public AssignmentDal(PTrackerEntities context)
    {
      objectContext = context;
    }

    public AssignmentDto Fetch(int projectId, int resourceId)
    {
      var result = (from r in objectContext.Assignments
                    where r.ProjectId == projectId && r.ResourceId == resourceId
                    select new AssignmentDto
                    {
                      ProjectId = r.ProjectId,
                      ResourceId = r.ResourceId,
                      Assigned = r.Assigned,
                      RoleId = r.RoleId,
                      LastChanged = r.LastChanged
                    }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("Assignment");
      return result;
    }

    public List<AssignmentDto> FetchForProject(int projectId)
    {
      var result = from r in objectContext.Assignments
                   where r.ProjectId == projectId
                   select new AssignmentDto
                   {
                     ProjectId = r.ProjectId,
                     ResourceId = r.ResourceId,
                     Assigned = r.Assigned,
                     RoleId = r.RoleId,
                     LastChanged = r.LastChanged
                   };
      return result.ToList();
    }

    public List<AssignmentDto> FetchForResource(int resourceId)
    {
      var result = from r in objectContext.Assignments
                   where r.ResourceId == resourceId
                   select new AssignmentDto
                   {
                     ProjectId = r.ProjectId,
                     ResourceId = r.ResourceId,
                     Assigned = r.Assigned,
                     RoleId = r.RoleId,
                     LastChanged = r.LastChanged
                   };
      return result.ToList();
    }

    public void Insert(AssignmentDto item)
    {
      var newItem = new Assignment
      {
        ProjectId = item.ProjectId,
        ResourceId = item.ResourceId,
        Assigned = item.Assigned,
        RoleId = item.RoleId
      };
      objectContext.AddToAssignments(newItem);
      objectContext.SaveChanges();
      item.LastChanged = newItem.LastChanged;
    }

    public void Update(AssignmentDto item)
    {
      var data = (from r in objectContext.Assignments
                  where r.ProjectId == item.ProjectId && r.ResourceId == item.ResourceId
                  select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Assignment");
      if (!data.LastChanged.Matches(item.LastChanged))
        throw new ConcurrencyException("Assignment");
      data.Assigned = item.Assigned;
      data.RoleId = item.RoleId;
      var count = objectContext.SaveChanges();
      if (count == 0)
        throw new UpdateFailureException("Assignment");
      item.LastChanged = data.LastChanged;
    }

    public void Delete(int projectId, int resourceId)
    {
      var data = (from r in objectContext.Assignments
                  where r.ProjectId == projectId && r.ResourceId == resourceId
                  select r).FirstOrDefault();
      if (data != null)
      {
        objectContext.Assignments.DeleteObject(data);
        objectContext.SaveChanges();
      }
    }
  }
}
