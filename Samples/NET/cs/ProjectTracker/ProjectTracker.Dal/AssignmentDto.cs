using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public class AssignmentDto
  {
    public int ProjectId { get; set; }
    public int ResourceId { get; set; }
    public int RoleId { get; set; }
    public DateTime Assigned { get; set; }
    public byte[] LastChanged { get; set; }
  }
}
