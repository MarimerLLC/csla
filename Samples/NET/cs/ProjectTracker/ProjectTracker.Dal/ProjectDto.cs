using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public class ProjectDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }
    public byte[] LastChanged { get; set; }
  }
}
