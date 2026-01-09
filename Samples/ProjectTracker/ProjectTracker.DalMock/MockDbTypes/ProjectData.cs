using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.DalMock.MockDbTypes
{
  public class ProjectData
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }
    public byte[] LastChanged { get; set; } = Array.Empty<byte>();
  }
}
