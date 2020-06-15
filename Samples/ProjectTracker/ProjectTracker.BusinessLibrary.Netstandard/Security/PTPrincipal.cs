using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Csla;

namespace ProjectTracker.Library.Security
{
  public static class PTPrincipal
  {
    public static event Action NewUser;

    public static async Task LoginAsync(string username, string password)
    {
      try
      {
        var credentials = DataPortal.Create<Credentials>(username, password);
        var validator = await DataPortal.FetchAsync<CredentialValidator>(credentials);
        var principal = validator.GetPrincipal();
        ApplicationContext.User = principal;
        NewUser?.Invoke();
      }
      catch
      {
        Logout();
      }
    }

    public static bool Login(string username, string password)
    {
      throw new NotSupportedException(nameof(Login));
    }

    public static void Logout()
    {
      Csla.ApplicationContext.User = new ClaimsPrincipal(new ClaimsIdentity());
      NewUser?.Invoke();
    }
  }
}
