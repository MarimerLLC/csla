using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IUserDal
  {
    UserDto Fetch(string username, string password);
    UserDto Fetch(string username);
  }
}
