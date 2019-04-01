using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IAssignmentDal
  {
    AssignmentDto Fetch(int projectId, int resourceId);
    List<AssignmentDto> FetchForProject(int projectId);
    List<AssignmentDto> FetchForResource(int resourceId);
    void Insert(AssignmentDto item);
    void Update(AssignmentDto item);
    void Delete(int projectId, int resourceId);
  }
}
