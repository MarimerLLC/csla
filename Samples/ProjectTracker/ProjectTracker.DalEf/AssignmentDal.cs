using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using Csla.Data;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class AssignmentDal : IAssignmentDal
  {
    public AssignmentDto Fetch(int projectId, int resourceId)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = (from r in ctx.ObjectContext.Assignments
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
    }

    public List<AssignmentDto> FetchForProject(int projectId)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = from r in ctx.ObjectContext.Assignments
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
    }

    public List<AssignmentDto> FetchForResource(int resourceId)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = from r in ctx.ObjectContext.Assignments
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
    }

    public void Insert(AssignmentDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var newItem = new Assignment
        {
          ProjectId = item.ProjectId,
          ResourceId = item.ResourceId,
          Assigned = item.Assigned,
          RoleId = item.RoleId
        };
        ctx.ObjectContext.AddToAssignments(newItem);
        ctx.ObjectContext.SaveChanges();
        item.LastChanged = newItem.LastChanged;
      }
    }

    public void Update(AssignmentDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Assignments
                    where r.ProjectId == item.ProjectId && r.ResourceId == item.ResourceId
                    select r).FirstOrDefault();
        if (data == null)
          throw new DataNotFoundException("Assignment");
        if (!data.LastChanged.Matches(item.LastChanged))
          throw new ConcurrencyException("Assignment");
        data.Assigned = item.Assigned;
        data.RoleId = item.RoleId;
        var count = ctx.ObjectContext.SaveChanges();
        if (count == 0)
          throw new UpdateFailureException("Assignment");
        item.LastChanged = data.LastChanged;
      }
    }

    public void Delete(int projectId, int resourceId)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Assignments
                    where r.ProjectId == projectId && r.ResourceId == resourceId
                    select r).FirstOrDefault();
        if (data != null)
        {
          ctx.ObjectContext.Assignments.DeleteObject(data);
          ctx.ObjectContext.SaveChanges();
        }
      }
    }
  }
}
