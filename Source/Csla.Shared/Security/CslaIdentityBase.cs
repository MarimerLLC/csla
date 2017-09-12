//-----------------------------------------------------------------------
// <copyright file="CslaIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Provides a base class to simplify creation of</summary>
//-----------------------------------------------------------------------
#if NET4
using Csla.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with CslaPrincipal.
  /// </summary>
  [Serializable]
  public abstract class CslaIdentityBase<T> :
    ReadOnlyBase<T>, ICheckRoles, ICslaIdentity
    where T : CslaIdentityBase<T>
  {
    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <returns></returns>
    public static UnauthenticatedIdentity UnauthenticatedIdentity()
    {
      return new Csla.Security.UnauthenticatedIdentity();
    }

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
      protected set { LoadProperty<string>(AuthenticationTypeProperty, value); }
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
      protected set { LoadProperty<bool>(IsAuthenticatedProperty, value); }
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
      protected set { LoadProperty<string>(NameProperty, value); }
    }
  }
}
#else
using Csla.Core;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Csla.Serialization.Mobile;
using System.ComponentModel;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with CslaPrincipal.
  /// </summary>
  [Serializable]
  public abstract class CslaIdentityBase<T> :
    ClaimsIdentity, ICheckRoles, ICslaIdentity
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

    object ICloneable.Clone()
    {
      throw new NotImplementedException();
    }

    public bool CanReadProperty(string propertyName)
    {
      throw new NotImplementedException();
    }

    public void Deserialized()
    {
      throw new NotImplementedException();
    }

    public bool CanWriteProperty(string propertyName)
    {
      throw new NotImplementedException();
    }

    public bool CanWriteProperty(IPropertyInfo property)
    {
      throw new NotImplementedException();
    }

    public bool CanReadProperty(IPropertyInfo property)
    {
      throw new NotImplementedException();
    }

    public bool CanExecuteMethod(string methodName)
    {
      throw new NotImplementedException();
    }

    public bool CanExecuteMethod(IMemberInfo method)
    {
      throw new NotImplementedException();
    }

    public void RuleStart(IPropertyInfo property)
    {
      throw new NotImplementedException();
    }

    public void RuleComplete(IPropertyInfo property)
    {
      throw new NotImplementedException();
    }

    public void RuleComplete(string property)
    {
      throw new NotImplementedException();
    }

    public void AllRulesComplete()
    {
      throw new NotImplementedException();
    }

    public void GetState(SerializationInfo info)
    {
      throw new NotImplementedException();
    }

    public void GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      throw new NotImplementedException();
    }

    public void SetState(SerializationInfo info)
    {
      throw new NotImplementedException();
    }

    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      throw new NotImplementedException();
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

    public event BusyChangedEventHandler BusyChanged;
    public event EventHandler<ErrorEventArgs> UnhandledAsyncException;
    public event PropertyChangedEventHandler PropertyChanged;
    public event PropertyChangingEventHandler PropertyChanging;

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

    public int Identity => throw new NotImplementedException();

    public bool IsBusy => throw new NotImplementedException();

    public bool IsSelfBusy => throw new NotImplementedException();

    #endregion
  }
}
#endif