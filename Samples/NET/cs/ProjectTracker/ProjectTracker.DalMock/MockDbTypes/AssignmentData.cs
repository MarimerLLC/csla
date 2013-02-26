using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.DalMock.MockDbTypes
{
  public class AssignmentData
  {
    public int ProjectId { get; set; }
    public int ResourceId { get; set; }
    public int RoleId { get; set; }
    public DateTime Assigned { get; set; }
    public byte[] LastChanged { get; set; }
  }
}
