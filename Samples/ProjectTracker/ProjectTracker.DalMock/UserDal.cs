using System.Linq;
using ProjectTracker.Dal;

namespace ProjectTracker.DalMock
{
  public class UserDal : IUserDal
  {
    public UserDto Fetch(string username, string password)
    {
      var result = (from r in MockDb.Users
                    where r.Username == username && r.Password == password
                    select new UserDto { Username = r.Username, Roles = r.Roles }).FirstOrDefault();
      return result;
    }

    public UserDto Fetch(string username)
    {
      var result = (from r in MockDb.Users
                    where r.Username == username
                    select new UserDto { Username = r.Username, Roles = r.Roles }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("User");
      return result;
    }
  }
}
