using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
  [Serializable]
  public class CustomPrincipal : System.Security.Principal.IPrincipal
  {
    private CustomIdentity _identity;

    public CustomPrincipal(CustomIdentity identity)
    {
      _identity = identity;
    }

    #region IPrincipal Members

    public System.Security.Principal.IIdentity Identity
    {
      get { return _identity; }
    }

    public bool IsInRole(string role)
    {
      return _identity.IsInRole(role);
    }

    #endregion
  }
}
