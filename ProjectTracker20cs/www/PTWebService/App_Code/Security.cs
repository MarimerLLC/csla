using System;
using System.Web;

public static class Security
{
  public static void UseAnonymous()
  {
    // setting an unauthenticated principal when running
    // under the VShost causes serialization issues
    // and isn't strictly necessary anyway
    if (UrlIsHostedByVS(HttpContext.Current.Request.Url))
      return;
    ProjectTracker.Library.Security.PTPrincipal.Logout();
  }

  public static void Login(CslaCredentials credentials)
  {
    if (string.IsNullOrEmpty(credentials.Username))
      throw new System.Security.SecurityException(
        "Valid credentials not provided");

    // set to unauthenticated principal
    ProjectTracker.Library.Security.PTPrincipal.Logout();

    ProjectTracker.Library.Security.PTPrincipal.Login(credentials.Username, credentials.Password);

    System.Security.Principal.IPrincipal principal =
      System.Threading.Thread.CurrentPrincipal;

    if (principal.Identity.IsAuthenticated)
    {
      // the user is valid - set up the HttpContext
      HttpContext.Current.User = principal;
    }
    else
    {
      // the user is not valid, raise an error
      throw new System.Security.SecurityException("Invalid user or password");
    }
  }

  public static bool UrlIsHostedByVS(Uri uri)
  {
    if (uri.Port >= 1024 && string.Compare(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase) == 0)
      return true;
    return false;
  }
}
