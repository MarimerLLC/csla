using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.DalMock.MockDbTypes
{
  public class UserData
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }
  }
}
