using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
  [Serializable]
  [Csla.Server.ObjectFactory("TestApp.CustomIdentityFactory, Permissions")]
  public class CustomIdentity : System.Security.Principal.IIdentity
  {
    private List<string> _roles;
    private string _name;
    private bool _isAuthenticated;

    public bool IsInRole(string role)
    {
      //return _roles.Contains(role);
      return HasPermission(role);
    }

    private List<string> _permissions;

    public bool HasPermission(string permission)
    {
      return _permissions.Contains(permission);
    }

    public CustomIdentity()
    {
      _roles = new List<string>();
    }

    public CustomIdentity(string name, List<string> roles)
    {
      _isAuthenticated = true;
      _name = name;
      _roles = roles;
    }

    public CustomIdentity(string name, List<string> roles, List<string> permissions)
    {
      _isAuthenticated = true;
      _name = name;
      _roles = roles;
      _permissions = permissions;
    }

    #region IIdentity Members

    public string AuthenticationType
    {
      get { return "Custom"; }
    }

    public bool IsAuthenticated
    {
      get { return _isAuthenticated; }
    }

    public string Name
    {
      get { return _name; }
    }

    #endregion
  }
}
