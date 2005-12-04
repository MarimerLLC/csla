using System;
using System.Security.Principal;

namespace ProjectTracker.Library.Security
{
  public class PTPrincipal : Csla.Security.BusinessPrincipalBase
  {

    private PTPrincipal(IIdentity identity)
      : base(identity) { }

    public static bool Login(string username, string password)
    {
      PTIdentity identity = PTIdentity.GetIdentity(username, password);
      if (identity.IsAuthenticated)
      {
        PTPrincipal principal = new PTPrincipal(identity);
        System.Threading.Thread.CurrentPrincipal = principal;
      }
      return identity.IsAuthenticated;
    }

    public static void Logout()
    {
      PTIdentity identity = PTIdentity.UnauthenticatedIdentity();
      PTPrincipal principal = new PTPrincipal(identity);
      System.Threading.Thread.CurrentPrincipal = principal;
    }

    public override bool IsInRole(string role)
    {
      PTIdentity identity = (PTIdentity)this.Identity;
      return identity.IsInRole(role);
    }

  }
}
