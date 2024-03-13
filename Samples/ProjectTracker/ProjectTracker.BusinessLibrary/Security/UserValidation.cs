using System;
using Csla;
using Csla.Core;
using ProjectTracker.Dal;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class UserValidation : CommandBase<UserValidation>
  {
    public static readonly PropertyInfo<bool> IsValidProperty = RegisterProperty<bool>(nameof(IsValid));
    public bool IsValid
    {
      get => ReadProperty(IsValidProperty);
      private set => LoadProperty(IsValidProperty, value);
    }

    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(nameof(Roles));
    public MobileList<string> Roles
    {
      get => ReadProperty(RolesProperty);
      private set => LoadProperty(RolesProperty, value);
    }

    [Execute]
    private void Execute(string username, [Inject] IUserDal dal)
    {
      var user = dal.Fetch(username);
      Roles = [.. user.Roles];
    }

    [Execute]
    private void Execute(string username, string password, [Inject] IUserDal dal)
    {
      var user = dal.Fetch(username, password);
      IsValid = (user is not null);
      if (IsValid)
        Roles = [.. user.Roles];
    }
  }
}
