using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public class RoleDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] LastChanged { get; set; }
  }
}
