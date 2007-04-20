using System;
using System.Web;
using System.Security.Principal;
using ProjectTracker.Library.Security;

public static class Security
{
  public static void UseAnonymous()
  {
    ProjectTracker.Library.Security.PTPrincipal.Logout();
  }
}
