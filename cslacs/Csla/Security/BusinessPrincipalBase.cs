using System;
using System.Security.Principal;

namespace Csla.Security
{
  /// <summary>
  /// Base class from which custom principal
  /// objects should inherit to operate
  /// properly with the data portal.
  /// </summary>
  [Serializable()]
  public class BusinessPrincipalBase : IPrincipal
  {
    private IIdentity _identity;

    /// <summary>
    /// Returns the user's identity object.
    /// </summary>
    public virtual IIdentity Identity
    {
      get { return _identity; }
    }

    /// <summary>
    /// Returns a value indicating whether the
    /// user is in a given role.
    /// </summary>
    /// <param name="role">Name of the role.</param>
    public virtual bool IsInRole(string role)
    {
      return false;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    protected BusinessPrincipalBase(IIdentity identity)
    {
      _identity = identity;
    }
  }
}
