using System;
using System.Security.Principal;
using Csla.Security;

namespace ProjectTracker.Library
{
  namespace Security
  {
    [Serializable()]
    public class PTPrincipal : BusinessPrincipalBase
    {
      private PTPrincipal(IIdentity identity)
        : base(identity)
      { }

      public static bool Login(string username, string password)
      {
        return SetPrincipal(PTIdentity.GetIdentity(username, password));
      }

      public static void LoadPrincipal(string username)
      {
        SetPrincipal(PTIdentity.GetIdentity(username));
      }

      private static bool SetPrincipal(PTIdentity identity)
      {
        if (identity.IsAuthenticated)
        {
          PTPrincipal principal = new PTPrincipal(identity);
          Csla.ApplicationContext.User = principal;
        }
        return identity.IsAuthenticated;
      }

      public static void Logout()
      {
        Csla.ApplicationContext.User = new UnauthenticatedPrincipal();
      }
    }
  }
}
