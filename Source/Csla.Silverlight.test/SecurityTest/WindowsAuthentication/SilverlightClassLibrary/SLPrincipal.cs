using System;
using System.Net;
using System.Security.Principal;
using Csla.Serialization;

namespace ClassLibrary
{
  [Serializable]
  public class SLPrincipal : Csla.Security.BusinessPrincipalBase
  {
    public SLPrincipal()
    { }

#if SILVERLIGHT

    public static void Login(EventHandler<EventArgs> completed)
    {
      Login(string.Empty, completed);
    }
    public static void Login(string role, EventHandler<EventArgs> completed)
    {
      if (!String.IsNullOrEmpty(role))
      {

      }

      SLWindowsIdentity.GetSLWindowsIdentity((o, e) =>
      {
        if (e.Error == null)
        {
          SetPrincipal(e.Object);
          if (completed != null)
          {
            completed(Csla.ApplicationContext.User, null);
          }
        }
        else
        {
          SetPrincipal(Csla.Security.CslaIdentity.UnauthenticatedIdentity());
          if (completed != null)
          {
            completed(Csla.ApplicationContext.User, null);
          }
        }
      });
    }
#endif

    private SLPrincipal(IIdentity identity) : base(identity) { }
    private static bool SetPrincipal(IIdentity identity)
    {
      SLPrincipal principal = new SLPrincipal(identity);
      Csla.ApplicationContext.User = principal;
      return identity.IsAuthenticated;
    }

    public static void Logout()
    {
      IIdentity identity = new SLWindowsIdentity();
      SLPrincipal principal = new SLPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      return ((SLWindowsIdentity)Identity).IsInRole(role);
    }
  }
}
