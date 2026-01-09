using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.DalMock.MockDbTypes
{
  public class UserData
  {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
  }
}
