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
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public MobileList<string> Roles
    {
      get => ReadProperty(RolesProperty) ?? new MobileList<string>();
      private set => LoadProperty(RolesProperty, value);
    }
#pragma warning restore CSLA0007

    [Execute]
    private void Execute(string username, [Inject] IUserDal dal)
    {
      var user = dal.Fetch(username);
      if (user is null)
      {
        Roles = new MobileList<string>();
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
        Roles = new MobileList<string>();
        return;
      }

      IsValid = true;
      Roles = new MobileList<string>(user.Roles);
    }
  }
}
