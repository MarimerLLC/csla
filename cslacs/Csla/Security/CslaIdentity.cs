using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with BusinessPrincipalBase.
  /// </summary>
  [Serializable()]
  public abstract partial class CslaIdentity : ReadOnlyBase<CslaIdentity>, IIdentity, ICheckRoles
  {
    #region UnauthenticatedIdentity

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <returns></returns>
    public static CslaIdentity UnauthenticatedIdentity()
    {
      return new Csla.Security.UnauthenticatedIdentity();
    }
    #endregion

    #region  IsInRole

    private static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty(new PropertyInfo<MobileList<string>>("Roles"));
    /// <summary>
    /// Gets or sets the list of roles for this user.
    /// </summary>
    protected MobileList<string> Roles
    {
      get { return GetProperty(RolesProperty); }
      set { LoadProperty(RolesProperty, value); }
    }

    bool ICheckRoles.IsInRole(string role)
    {
      var roles = GetProperty<MobileList<string>>(RolesProperty);
      if (roles != null)
        return roles.Contains(role);
      else
        return false;
    }

    #endregion

    #region  IIdentity

    private static readonly PropertyInfo<string> AuthenticationTypeProperty = 
      RegisterProperty<string>(new PropertyInfo<string>("AuthenticationType", "Authentication type", "Csla"));
    /// <summary>
    /// Gets the authentication type for this identity.
    /// </summary>
    public string AuthenticationType
    {
      get { return GetProperty<string>(AuthenticationTypeProperty); }
      protected set { LoadProperty<string>(AuthenticationTypeProperty, value); }
    }

    private static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(new PropertyInfo<bool>("IsAuthenticated"));
    /// <summary>
    /// Gets a value indicating whether this identity represents
    /// an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
      get { return GetProperty<bool>(IsAuthenticatedProperty); }
      protected set { LoadProperty<bool>(IsAuthenticatedProperty, value); }
    }

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name"));
    /// <summary>
    /// Gets the username value.
    /// </summary>
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      protected set { LoadProperty<string>(NameProperty, value); }
    }

    #endregion

  }

}
