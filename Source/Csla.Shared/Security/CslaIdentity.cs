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
  /// a .NET identity object for use with CslaPrincipal.
  /// </summary>
  [Serializable]
  public abstract class CslaIdentity : CslaIdentityBase<CslaIdentity>,
    ICslaIdentity
  {
#if (ANDROID || IOS) || NETFX_CORE
    /// <summary>
    /// Retrieves an instance of the identity
    /// object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object.
    /// </typeparam>
    /// <param name="completed">
    /// Method called when the operation is
    /// complete.
    /// </param>
    /// <param name="criteria">
    /// Criteria object for the query.
    /// </param>
    public static void GetCslaIdentity<T>(EventHandler<DataPortalResult<T>> completed, object criteria)
      where T : CslaIdentity
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(criteria);
    }
#else
    /// <summary>
    /// Invokes the data portal to get an instance of
    /// the identity object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the CslaIdentity subclass to retrieve.
    /// </typeparam>
    /// <param name="criteria">
    /// Object containing the user's credentials.
    /// </param>
    /// <returns></returns>
    public static T GetCslaIdentity<T>(object criteria) 
      where T : CslaIdentity
    {
      return DataPortal.Fetch<T>(criteria);
    }
#endif
  }

  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with CslaPrincipal.
  /// </summary>
  [Serializable]
  public abstract class CslaIdentityBase<T> :
    ReadOnlyBase<T>, IIdentity, ICheckRoles,
    ICslaIdentity
    where T : CslaIdentityBase<T>
  {
    #region UnauthenticatedIdentity

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <returns></returns>
    public static UnauthenticatedIdentity UnauthenticatedIdentity()
    {
      return new Csla.Security.UnauthenticatedIdentity();
    }
    #endregion

    #region  IsInRole

    /// <summary>
    /// Property info for Roles property
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

    /// <summary>
    /// Property info for Authentication property
    /// </summary>
    public static readonly PropertyInfo<string> AuthenticationTypeProperty = RegisterProperty<string>(c => c.AuthenticationType, "AuthenticationType", "Csla");

    /// <summary>
    /// Gets the authentication type for this identity.
    /// </summary>
    public string AuthenticationType
    {
      get { return GetProperty<string>(AuthenticationTypeProperty); }
#if IOS
      set { LoadProperty<string>(AuthenticationTypeProperty, value); }
#else
      protected set { LoadProperty<string>(AuthenticationTypeProperty, value); }
#endif
    }


    /// <summary>
    /// 
    /// </summary>
    public static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(c => c.IsAuthenticated);

    /// <summary>
    /// Gets a value indicating whether this identity represents
    /// an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
      get { return GetProperty<bool>(IsAuthenticatedProperty); }
#if IOS
      set { LoadProperty<bool>(IsAuthenticatedProperty, value); }
#else
      protected set { LoadProperty<bool>(IsAuthenticatedProperty, value); }
#endif
    }

    /// <summary>
    /// Property info for Name property
    /// </summary>
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);

    /// <summary>
    /// Gets the username value.
    /// </summary>
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
#if IOS
      set { LoadProperty<string>(NameProperty, value); }
#else
      protected set { LoadProperty<string>(NameProperty, value); }
#endif
    }

    #endregion
  }
}