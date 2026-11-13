using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IUserDal
  {
    /// <summary>
    /// Gets the user, or null if the username or password is wrong.
    /// </summary>
    UserDto? Fetch(string username, string password);
    /// <summary>
    /// Gets the user, or null if there is no such user.
    /// </summary>
    UserDto? Fetch(string username);
  }
}
