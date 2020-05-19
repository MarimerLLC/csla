using System;
using Csla;
using Csla.Core;
using Csla.Security;
using ProjectTracker.Dal;

namespace ProjectTracker.BusinessLibrary.Security
{
  [Serializable]
  public class CredentialValidator : ReadOnlyBase<CredentialValidator>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      private set => LoadProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<string> AuthenticationTypeProperty = RegisterProperty<string>(nameof(AuthenticationType));
    public string AuthenticationType
    {
      get => GetProperty(AuthenticationTypeProperty);
      private set => LoadProperty(AuthenticationTypeProperty, value);
    }

    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(nameof(Roles));
    public MobileList<string> Roles
    {
      get => GetProperty(RolesProperty);
      private set => LoadProperty(RolesProperty, value);
    }

    [Fetch]
    private void Fetch(Credentials credentials, [Inject] IUserDal dal)
    {
      var data = dal.Fetch(credentials.Username, credentials.Password);
      if (data != null)
      {
        AuthenticationType = "Custom";
        Name = data.Username;
        Roles = new MobileList<string>(data.Roles);
      }
    }
  }
}
