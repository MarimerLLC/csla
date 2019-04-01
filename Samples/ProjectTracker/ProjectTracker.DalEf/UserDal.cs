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
        return Fetch(username);
      }
      else
      {
        throw new DataNotFoundException("User");
      }
    }

    public UserDto Fetch(string username)
    {
      try
      {
        var user = Membership.GetUser(username);
        var result = new UserDto { Username = user.UserName };
        result.Roles = Roles.Provider.GetRolesForUser(result.Username);
        return result;
      }
      catch (Exception ex)
      {
        throw new DataNotFoundException("User", ex);
      }
    }
  }
}
