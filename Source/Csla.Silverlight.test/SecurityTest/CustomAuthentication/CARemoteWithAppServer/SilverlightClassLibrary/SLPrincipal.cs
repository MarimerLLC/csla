using System;
using System.Net;
using System.Security.Principal;
using Csla.DataPortalClient;

namespace ClassLibrary
{
  public class SLPrincipal : Csla.Security.BusinessPrincipalBase
  {
    private SLPrincipal(IIdentity identity)
      : base(identity)
    { }

#if SILVERLIGHT
    public static void Login(string username, string password, EventHandler<EventArgs> completed)
      {        
        SLIdentity.GetIdentity(username, password, (o, e) =>
        {
          SetPrincipal(e.Object);
          completed(null, new EventArgs());
        });
      }
#endif

    private static void SetPrincipal(Csla.Security.CslaIdentity identity)
    {
      if (identity != null && identity.IsAuthenticated)
        Csla.ApplicationContext.User = new SLPrincipal(identity);
      else
        Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();
    }

    public static void Logout()
    {
      Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();
    }
  }
}
