using System;
using System.Security.Claims;
using Csla;
using Csla.Core;
using Csla.Security;
using ProjectTracker.Dal;

namespace ProjectTracker.Library.Security
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

    public ClaimsPrincipal GetPrincipal()
    {
      var identity = new ClaimsIdentity(AuthenticationType);
      if (!string.IsNullOrWhiteSpace(Name))
      {
        identity.AddClaim(new Claim(ClaimTypes.Name, Name));
        if (Roles != null)
          foreach (var item in Roles)
            identity.AddClaim(new Claim(ClaimTypes.Role, item));
      }
      return new ClaimsPrincipal(identity);
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
