using System;
using System.Security.Principal;
using Csla;
using Csla.Security;
using Csla.Serialization;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class PTPrincipal : CslaPrincipal
  {
    public PTPrincipal()
    { }

    protected PTPrincipal(IIdentity identity)
      : base(identity)
    { }

#if !__ANDROID__
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
    public static System.Threading.Tasks.Task LoginAsync(string username, string password)
    {
        var tcs = new System.Threading.Tasks.TaskCompletionSource<PTPrincipal>();

        PTIdentity.GetPTIdentity(username, password, (o, e) =>
        {
            if (e.Error == null && e.Object != null)
            {
                SetPrincipal(e.Object);
                tcs.SetResult(null);
            }
            else
            {
                Logout();
                if (e.Error != null) 
                    tcs.SetException(e.Error.InnerException);
                else
                    tcs.SetCanceled();
            }
        });
        return tcs.Task;
    }
#endif

#if !SILVERLIGHT && !NETFX_CORE
    public static bool Login(string username, string password)
    {
      var identity = PTIdentity.GetPTIdentity(username, password);
      return SetPrincipal(identity);
    }

    public static bool Load(string username)
    {
      var identity = PTIdentity.GetPTIdentity(username);
      return SetPrincipal(identity);
    }
#endif

    private static bool SetPrincipal(IIdentity identity)
    {
      if (identity.IsAuthenticated)
      {
        PTPrincipal principal = new PTPrincipal(identity);
        Csla.ApplicationContext.User = principal;
      }
      OnNewUser();
      return identity.IsAuthenticated;
    }

    public static void Logout()
    {
      Csla.ApplicationContext.User = new UnauthenticatedPrincipal();
      OnNewUser();
    }

    public static event Action NewUser;
    private static void OnNewUser()
    {
      if (NewUser != null)
        NewUser();
    }
  }
}
