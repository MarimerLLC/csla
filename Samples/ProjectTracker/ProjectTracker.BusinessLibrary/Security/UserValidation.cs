using Csla;
using Csla.Core;
using ProjectTracker.Dal;

namespace ProjectTracker.Library.Security
{
  [CslaImplementProperties]
  public partial class UserValidation : CommandBase<UserValidation>
  {
    public partial bool IsValid { get; private set; }

    public partial MobileList<string> Roles { get; private set; }

    [Execute]
    private void Execute(string username, [Inject] IUserDal dal)
    {
      UserDto? user;
      try
      {
        user = dal.Fetch(username);
      }
      catch (DataNotFoundException)
      {
        // the DAL throws when there is no such user
        user = null;
      }
      if (user is null)
      {
        Roles = [];
        IsValid = false;
        return;
      }

      Roles = new MobileList<string>(user.Roles);
      IsValid = true;
    }

    [Execute]
    private void Execute(string username, string password, [Inject] IUserDal dal)
    {
      UserDto? user;
      try
      {
        user = dal.Fetch(username, password);
      }
      catch (DataNotFoundException)
      {
        // the DAL throws when the username or password is wrong
        user = null;
      }
      if (user is null)
      {
        IsValid = false;
        Roles = [];
        return;
      }

      IsValid = true;
      Roles = new MobileList<string>(user.Roles);
    }
  }
}
