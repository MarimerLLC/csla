using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Core;
using Csla.Serialization;
using ProjectTracker.Dal;

namespace ProjectTracker.BusinessLibrary.Security
{
  [Serializable]
  public class UserValidation : ReadOnlyBase<UserValidation>
  {
    public static readonly PropertyInfo<string> UserNameProperty = RegisterProperty<string>(nameof(UserName));
    [Required]
    public string UserName
    {
      get => GetProperty(UserNameProperty);
      set => LoadProperty(UserNameProperty, value);
    }

    public static readonly PropertyInfo<string> PasswordProperty = RegisterProperty<string>(nameof(Password));
    [Required]
    public string Password
    {
      get => GetProperty(PasswordProperty);
      set => LoadProperty(PasswordProperty, value);
    }

    public static readonly PropertyInfo<bool> IsValidProperty = RegisterProperty<bool>(nameof(IsValid));
    public bool IsValid
    {
      get => GetProperty(IsValidProperty);
      private set => LoadProperty(IsValidProperty, value);
    }

    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(nameof(Roles));
    public MobileList<string> Roles
    {
      get => GetProperty(RolesProperty);
      set => LoadProperty(RolesProperty, value);
    }

    [Execute]
    private void Execute([Inject] IUserDal dal)
    {
      var user = dal.Fetch(UserName, Password);
      IsValid = (user is not null);
      Roles = new();
      foreach (var item in user.Roles)
        Roles.Add(item);
    }
  }
}
