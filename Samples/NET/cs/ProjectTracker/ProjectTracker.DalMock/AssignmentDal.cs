using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using ProjectTracker.DalMock.MockDbTypes;

namespace ProjectTracker.DalMock
{
  public class AssignmentDal : IAssignmentDal
  {
    public AssignmentDto Fetch(int projectId, int resourceId)
    {
      var result = (from r in MockDb.Assignments
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
      var result = from r in MockDb.Assignments
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
      var result = from r in MockDb.Assignments
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
      item.LastChanged = MockDb.GetTimeStamp();
      var newItem = new AssignmentData
      {
        ProjectId = item.ProjectId,
        ResourceId = item.ResourceId,
        Assigned = item.Assigned,
        RoleId = item.RoleId,
        LastChanged = item.LastChanged
      };
      MockDb.Assignments.Add(newItem);
    }

    public void Update(AssignmentDto item)
    {
      var data = (from r in MockDb.Assignments
                     where r.ProjectId == item.ProjectId && r.ResourceId == item.ResourceId
                     select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Assignment");
      if (!data.LastChanged.Matches(item.LastChanged))
        throw new ConcurrencyException("Assignment");
      item.LastChanged = MockDb.GetTimeStamp();
      data.Assigned = item.Assigned;
      data.RoleId = item.RoleId;
      data.LastChanged = item.LastChanged;
    }

    public void Delete(int projectId, int resourceId)
    {
      var data = (from r in MockDb.Assignments
                  where r.ProjectId == projectId && r.ResourceId == resourceId
                  select r).FirstOrDefault();
      if (data != null)
        MockDb.Assignments.Remove(data);
    }
  }
}
