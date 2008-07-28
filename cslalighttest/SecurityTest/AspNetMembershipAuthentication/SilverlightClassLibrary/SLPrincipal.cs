using System;
using System.Net;
using System.Security.Principal;

namespace SilverlightClassLibrary
{
  public class SLPrincipal : Csla.Security.BusinessPrincipalBase
  {
    private SLPrincipal(IIdentity identity)
      : base(identity)
    { }

#if SILVERLIGHT
    public static void Login(string username, string password, string roles, EventHandler<EventArgs> completed)
    {
      SLMembershipIdentity.GetMembershipIdentity<SLMembershipIdentity>((o, e) =>
      {
        bool result = SetPrincipal(e.Object);
        e.Object.SetRoles(roles);
        completed(null, new LoginEventArgs(result));
      }, username, password, true);
    }
#endif

    private static bool SetPrincipal(IIdentity identity)
    {
      if (identity.IsAuthenticated)
      {
        SLPrincipal principal = new SLPrincipal(identity);
        Csla.ApplicationContext.User = principal;
      }
      else
      {
        identity = new SLMembershipIdentity();
        SLPrincipal principal = new SLPrincipal(new SLMembershipIdentity());
        Csla.ApplicationContext.User = principal;
      }
      return identity.IsAuthenticated;
    }

    public static void Logout()
    {
      IIdentity identity = new SLMembershipIdentity();
      SLPrincipal principal = new SLPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      return ((SLMembershipIdentity)base.Identity).InRole(role);
    }

    public class LoginEventArgs : EventArgs
    {

      private bool _loginSucceded;

      public bool LoginSucceded
      {
        get
        {
          return _loginSucceded;
        }
      }

      public LoginEventArgs(bool loginSucceded)
      {
        _loginSucceded = loginSucceded;
      }
    }
  }
}
