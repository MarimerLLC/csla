using System;
using System.Security.Principal;
using Csla.Security;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  namespace Security
  {
    [Serializable]
    public class PTPrincipal : CslaPrincipal
    {
      private PTPrincipal(IIdentity identity)
        : base(identity)
      { }

#if SILVERLIGHT
      public static void BeginLogin(string username, string password)
      {
        PTIdentity.GetPTIdentity(username, password, (o, e) =>
          {
            if (e.Error == null && e.Object != null)
              SetPrincipal(e.Object);
            else
              Logout();
          });
      }
#else
      public static bool Login(string username, string password)
      {
        return SetPrincipal(PTIdentity.GetIdentity(username, password));
      }

      public static void LoadPrincipal(string username)
      {
        SetPrincipal(PTIdentity.GetIdentity(username));
      }
#endif

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
