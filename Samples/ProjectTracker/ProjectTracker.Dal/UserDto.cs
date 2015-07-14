using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public class UserDto
  {
    public string Username { get; set; }
    public string[] Roles { get; set; }
  }
}
