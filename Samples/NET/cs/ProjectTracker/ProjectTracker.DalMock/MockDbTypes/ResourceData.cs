using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.DalMock.MockDbTypes
{
  public class ResourceData
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[] LastChanged { get; set; }
  }
}
