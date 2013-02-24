using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
  public static class User
  {
    public static string Name
    {
      get { return Csla.ApplicationContext.User.Identity.Name; }
    }

    public static bool IsAuthenticated
    {
      get { return Csla.ApplicationContext.User.Identity.IsAuthenticated; }
    }

    public static string AuthenticationType
    {
      get { return Csla.ApplicationContext.User.Identity.AuthenticationType; }
    }

    public static bool IsInRole(string role)
    {
      return Csla.ApplicationContext.User.IsInRole(role);
    }

    public static bool HasPermission(string permission)
    {
      var ident = Csla.ApplicationContext.User.Identity as CustomIdentity;
      if (ident != null)
        return ident.HasPermission(permission);
      else
        return IsInRole(permission);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static bool IsInRoleProvider(
      System.Security.Principal.IPrincipal principal, string role)
    {
      return User.HasPermission(role);
    }
  }
}
