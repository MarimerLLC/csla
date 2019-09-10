using System;
using System.Security.Principal;
using Csla.Security;

namespace Rolodex.Business.Security
{
  [Serializable]
  public class RolodexPrincipal : CslaPrincipal
  {
    private RolodexPrincipal(IIdentity identity)
      : base(identity)
    {
    }

    public RolodexPrincipal() : base()
    {
    }

    public static void Login(string username, string password)
    {
      RolodexIdentity identity = RolodexIdentity.GetIdentity(username, password);
      SetPrincipal(identity);
    }

    private static void SetPrincipal(CslaIdentity identity)
    {
      RolodexPrincipal principal = new RolodexPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public static void Logout()
    {
      var identity = RolodexIdentity.UnauthenticatedIdentity();
      RolodexPrincipal principal = new RolodexPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      return ((ICheckRoles) base.Identity).IsInRole(role);
    }
  }
}