//-----------------------------------------------------------------------
// <copyright file="CslaIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Provides a base class to simplify creation of</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using System.Reflection;
using Csla.Core;
using Csla.Reflection;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with BusinessPrincipalBase.
  /// </summary>
  [Serializable]
  public abstract partial class CslaIdentity : CslaIdentityBase<CslaIdentity>
  {
    private static bool _forceInit;
  }

  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with BusinessPrincipalBase.
  /// </summary>
  [Serializable]
  public abstract class CslaIdentityBase<T> : 
    ReadOnlyBase<T>, IIdentity, ICheckRoles
    where T : CslaIdentityBase<T>
  {
#if SILVERLIGHT
    private static bool _forceInit;

    /// <summary>
    /// Create an instance of the object.
    /// </summary>
    public CslaIdentityBase()
    {
      _forceInit = !_forceInit;
    }

    /// <summary>
    /// Invoked when the object is deserialized.
    /// </summary>
    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = _forceInit && false;
      base.OnDeserialized(context);
    }
#endif

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

    /// <summary>
    /// Gets or sets the list of roles for this user.
    /// </summary>
    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(c => c.Roles);
    /// <summary>
    /// Gets or sets the list of roles for this user.
    /// </summary>
    public MobileList<string> Roles
    {
      get { return GetProperty(RolesProperty); }
      protected set { LoadProperty(RolesProperty, value); }
    }

    bool ICheckRoles.IsInRole(string role)
    {
      var roles = ReadProperty<MobileList<string>>(RolesProperty);
      if (roles != null)
        return roles.Contains(role);
      else
        return false;
    }

    #endregion

    #region  IIdentity

    public static readonly PropertyInfo<string> AuthenticationTypeProperty = RegisterProperty<string>(c => c.AuthenticationType, "AuthenticationType", "Csla");
    /// <summary>
    /// Gets the authentication type for this identity.
    /// </summary>
    public string AuthenticationType
    {
      get { return GetProperty<string>(AuthenticationTypeProperty); }
      protected set { LoadProperty<string>(AuthenticationTypeProperty, value); }
    }

    public static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(c => c.IsAuthenticated);
    /// <summary>
    /// Gets a value indicating whether this identity represents
    /// an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
      get { return GetProperty<bool>(IsAuthenticatedProperty); }
      protected set { LoadProperty<bool>(IsAuthenticatedProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
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