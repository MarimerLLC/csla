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
      var user = dal.Fetch(username);
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
      var user = dal.Fetch(username, password);
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
