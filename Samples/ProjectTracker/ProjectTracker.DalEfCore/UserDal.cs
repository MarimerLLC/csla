using System.Collections.Generic;
using System.Linq;
using ProjectTracker.Dal;

namespace ProjectTracker.DalEfCore
{
  public class UserDal : IUserDal
  {
    private static readonly List<UserData> Users = new List<UserData>
      {
        new UserData { Username = "manager", Password = "manager", Roles = new string[] { "ProjectManager" }},
        new UserData { Username = "admin", Password = "admin", Roles = new string[] { "Administrator" }}
      };

    public UserDto Fetch(string username, string password)
    {
      var result = (from r in Users
                    where r.Username == username && r.Password == password
                    select new UserDto { Username = r.Username, Roles = r.Roles }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("User");
      return result;
    }

    public UserDto Fetch(string username)
    {
      var result = (from r in Users
                    where r.Username == username
                    select new UserDto { Username = r.Username, Roles = r.Roles }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("User");
      return result;
    }
  }

  public class UserData
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }
  }
}
