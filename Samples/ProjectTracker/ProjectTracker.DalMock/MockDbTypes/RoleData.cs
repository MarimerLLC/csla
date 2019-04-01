using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.DalMock.MockDbTypes
{
  public class RoleData
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] LastChanged { get; set; }
  }
}
