using System;
using System.Security.Principal;
using System.Threading;

namespace CSLA.Security
{
  /// <summary>
  /// Implements a custom Principal class that is used by
  /// CSLA .NET for table-based security.
  /// </summary>
  [Serializable()]
  public class BusinessPrincipal : IPrincipal
  {
    BusinessIdentity _identity;

    #region IPrincipal

    /// <summary>
    /// Implements the Identity property defined by IPrincipal.
    /// </summary>
    IIdentity IPrincipal.Identity
    {
      get
      {
        return _identity;
      }
    }

    /// <summary>
    /// Implements the IsInRole property defined by IPrincipal.
    /// </summary>
    bool IPrincipal.IsInRole(string role)
    {
      return _identity.IsInRole(role);
    }

    #endregion

    #region Login process

    /// <summary>
    /// Initiates a login process using custom CSLA .NET security.
    /// </summary>
    /// <remarks>
    /// As described in the book, this invokes a login process using
    /// a table-based authentication scheme and a list of roles in
    /// the database tables. By replacing the code in
    /// <see cref="T:CSLA.Security.BusinessIdentity" /> you can easily
    /// adapt this scheme to authenticate the user against any database
    /// or other scheme.
    /// </remarks>
    /// <param name="Username">The user's username.</param>
    /// <param name="Password">The user's password.</param>
    public static void Login(string username, string password)
    {
      BusinessPrincipal p = new BusinessPrincipal(username, password);
    }

    private BusinessPrincipal(string username, string password)
    {
      AppDomain currentdomain = Thread.GetDomain();
      currentdomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal);

      IPrincipal oldPrincipal = Thread.CurrentPrincipal;
      Thread.CurrentPrincipal = this;

      try
      {
        if(!(oldPrincipal.GetType() == typeof(BusinessPrincipal)))
          currentdomain.SetThreadPrincipal(this);
      }                                                                                        
      catch
      {
        // failed, but we don't care because there's nothing
        // we can do in this case
      }

      // load the underlying identity object that tells whether
      // we are really logged in, and if so will contain the 
      // list of roles we belong to
      _identity = BusinessIdentity.LoadIdentity(username, password);
    }

    #endregion

  }
}
