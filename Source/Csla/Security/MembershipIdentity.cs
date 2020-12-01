//-----------------------------------------------------------------------
// <copyright file="MembershipIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a .NET identity object that automatically</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Implements a .NET identity object that automatically
  /// authenticates against the ASP.NET membership provider.
  /// </summary>
  [Csla.Server.MobileFactory("Csla.Web.Security.IdentityWebFactory,Csla.Web")]
  [Csla.Server.ObjectFactory("Csla.Web.Security.IdentityAppFactory,Csla.Web")]
  [Serializable]
  public class MembershipIdentity : ReadOnlyBase<MembershipIdentity>, IIdentity, ICheckRoles
  {
    private static int _forceInit = 0;

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    protected MembershipIdentity()
    {
      _forceInit = _forceInit + 0;
    }

    /// <summary>
    /// Method invoked when the object is deserialized.
    /// </summary>
    /// <param name="context">Serialization context.</param>
    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = _forceInit + 0;
      base.OnDeserialized(context);
    }

    /// <summary>
    /// Authenticates the user's credentials against the ASP.NET
    /// membership provider.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object (subclass of MembershipIdentity) to retrieve.
    /// </typeparam>
    /// <param name="userName">Username to authenticate.</param>
    /// <param name="password">Password to authenticate.</param>
    /// <returns></returns>
    public static T GetMembershipIdentity<T>(string userName, string password) where T : MembershipIdentity
    {
      return DataPortal.Fetch<T>(new Criteria(userName, password, typeof(T)));
    }

    /// <summary>
    /// Gets or sets a list of roles for this user.
    /// </summary>
    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(c => c.Roles);
    /// <summary>
    /// Gets or sets a list of roles for this user.
    /// </summary>
    protected internal MobileList<string> Roles
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

    /// <summary>
    /// Gets the authentication type for this identity.
    /// </summary>
    public static readonly PropertyInfo<string> AuthenticationTypeProperty = RegisterProperty<string>(c => c.AuthenticationType);
    /// <summary>
    /// Gets the authentication type for this identity.
    /// </summary>
    public string AuthenticationType
    {
      get { return GetProperty(AuthenticationTypeProperty); }
      protected set { LoadProperty(AuthenticationTypeProperty, value); }
    }

    /// <summary>
    /// Gets a value indicating whether this identity represents
    /// an authenticated user.
    /// </summary>
    public static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(c => c.IsAuthenticated);
    /// <summary>
    /// Gets a value indicating whether this identity represents
    /// an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
      get { return GetProperty(IsAuthenticatedProperty); }
      protected set { LoadProperty(IsAuthenticatedProperty, value); }
    }

    /// <summary>
    /// Gets the username value.
    /// </summary>
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    /// <summary>
    /// Gets the username value.
    /// </summary>
    public string Name
    {
      get { return GetProperty(NameProperty); }
      protected set { LoadProperty(NameProperty, value); }
    }

    /// <summary>
    /// Override this method in a subclass to load custom
    /// data beyond the automatically loaded values from
    /// the membership and role providers.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public virtual void LoadCustomData() { }

    /// <summary>
    /// Criteria object containing the user credentials
    /// to be authenticated.
    /// </summary>
    [Serializable()]
    public class Criteria : Csla.Core.MobileObject
    {
      private string _name;
      private string _password;
      private string _membershipIdentityType;
      private Criteria() { }
      /// <summary>
      /// Gets or sets the username.
      /// </summary>
      public string Name
      {
        get { return _name; }
        set { _name = value; }
      }

      /// <summary>
      /// Gets or sets the password.
      /// </summary>
      public string Password
      {
        get { return _password; }
        set { _password = value; }
      }

      /// <summary>
      /// Gets or sets the membership identity type.
      /// </summary>
      public string MembershipIdentityType
      {
        get { return _membershipIdentityType; }
        set { _membershipIdentityType = value; }
      }

      /// <summary>
      /// Creates an instance of the class.
      /// </summary>
      /// <param name="name">Username.</param>
      /// <param name="password">Password.</param>
      /// <param name="membershipIdentityType">Membership identity type.</param>
      public Criteria(string name, string password, Type membershipIdentityType)
      {
        Name = name;
        Password = password;
        MembershipIdentityType = membershipIdentityType.AssemblyQualifiedName;
      }

      /// <summary>
      /// Override this method to get custom field values
      /// from the serialization stream.
      /// </summary>
      /// <param name="info">Serialization info.</param>
      /// <param name="mode">Serialization mode.</param>
      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        info.AddValue("MembershipIdentity.Criteria.Name", Name);
        info.AddValue("MembershipIdentity.Criteria.Password", Password);
        info.AddValue("MembershipIdentity.Criteria.MembershipIdentityType", MembershipIdentityType);
        base.OnGetState(info, mode);
      }

      /// <summary>
      /// Override this method to set custom field values
      /// into the serialization stream.
      /// </summary>
      /// <param name="info">Serialization info.</param>
      /// <param name="mode">Serialization mode.</param>
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        base.OnSetState(info, mode);
        Name = info.GetValue<string>("MembershipIdentity.Criteria.Name");
        Password = info.GetValue<string>("MembershipIdentity.Criteria.Password");
        MembershipIdentityType = info.GetValue<string>("MembershipIdentity.Criteria.MembershipIdentityType");
      }
    }
  }
}