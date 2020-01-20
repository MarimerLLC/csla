using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Csla.Security;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class PTPrincipal : CslaClaimsPrincipal
  {
    public PTPrincipal()
    { }

    protected PTPrincipal(ICslaIdentity identity)
      : base(identity)
    { }

    public static async Task LoginAsync(string username, string password)
    {
      try
      {
        var identity = await PTIdentity.GetPTIdentityAsync(username, password);
        SetPrincipal(identity);
      }
      catch
      {
        Logout();
      }
    }

    public static bool Login(string username, string password)
    {
      var identity = PTIdentity.GetPTIdentity(username, password);
      return SetPrincipal(identity);
    }

    public static bool Load(string username)
    {
      var current = Csla.ApplicationContext.User;
      if (current != null && current.Identity != null && current.Identity.Name == username)
        return true;

      var identity = PTIdentity.GetPTIdentity(username);
      return SetPrincipal(identity);
    }

    public static async Task<bool> LoadAsync(string username)
    {
      var current = Csla.ApplicationContext.User;
      if (current != null && current.Identity != null && current.Identity.Name == username)
        return true;

      var identity = await PTIdentity.GetPTIdentityAsync(username);
      return SetPrincipal(identity);
    }

    private static bool SetPrincipal(ICslaIdentity identity)
    {
      if (identity.IsAuthenticated)
      {
        PTPrincipal principal = new PTPrincipal(identity);
        Csla.ApplicationContext.User = principal;
      }
      OnNewUser();
      return identity.IsAuthenticated;
    }

    private static bool SetPrincipal(IPrincipal principal)
    {
      Csla.ApplicationContext.User = principal;
      OnNewUser();
      return principal.Identity.IsAuthenticated;
    }

    public static void Logout()
    {
      var identity = Csla.DataPortal.Create<PTIdentity>();
      Csla.ApplicationContext.User = new PTPrincipal(identity);
      OnNewUser();
    }

    public static event Action NewUser;
    private static void OnNewUser()
    {
      NewUser?.Invoke();
    }
  }
}
