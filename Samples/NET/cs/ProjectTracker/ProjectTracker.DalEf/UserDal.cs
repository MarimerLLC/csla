using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using System.Web.Security;

namespace ProjectTracker.DalEf
{
  public class UserDal : IUserDal
  {
    public UserDto Fetch(string username, string password)
    {
      if (Membership.ValidateUser(username, password))
      {
        var result = new UserDto { Username = username };
        result.Roles = Roles.GetRolesForUser(result.Username);
        return result;
      }
      else
      {
        throw new DataNotFoundException("User");
      }
    }
  }
}
