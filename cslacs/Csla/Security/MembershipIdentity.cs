using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
using Csla.Silverlight;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Implements a .NET identity object that automatically
  /// authenticates against the ASP.NET membership provider.
  /// </summary>
  [Serializable()]
  [MobileFactory("Csla.Security.IdentityFactory,Csla", "FetchMembershipIdentity")]
  public partial class MembershipIdentity : ReadOnlyBase<MembershipIdentity>, IIdentity, ICheckRoles
  {
    #region Constructor, Helper Setter

    private static int _forceInit = 0;

    #endregion

    #region  IsInRole

    private static readonly PropertyInfo<MobileList<string>> RolesProperty =
      RegisterProperty(new PropertyInfo<MobileList<string>>("Roles"));
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

    #endregion

    #region  IIdentity

    private static readonly PropertyInfo<string> AuthenticationTypeProperty = RegisterProperty<string>(new PropertyInfo<string>("AuthenticationType"));
    /// <summary>
    /// Gets the authentication type for this identity.
    /// </summary>
    public string AuthenticationType
    {
      get
      {
        string authenticationType = GetProperty<string>(AuthenticationTypeProperty);
        if (authenticationType == null)
        {
          authenticationType = "Csla";
          LoadProperty<string>(AuthenticationTypeProperty, authenticationType);
          return authenticationType;
        }
        else
        {
          return authenticationType;
        }
      }
      protected internal set
      {
        LoadProperty<string>(AuthenticationTypeProperty, value);
      }
    }

    private static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(new PropertyInfo<bool>("IsAuthenticated"));
    /// <summary>
    /// Gets a value indicating whether this identity represents
    /// an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
      get
      {
        return GetProperty<bool>(IsAuthenticatedProperty);
      }
      protected internal set
      {
        LoadProperty<bool>(IsAuthenticatedProperty, value);
      }
    }

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name"));
    /// <summary>
    /// Gets the username value.
    /// </summary>
    public string Name
    {
      get
      {
        return GetProperty<string>(NameProperty);
      }
      protected internal set
      {
        LoadProperty<string>(NameProperty, value);
      }
    }

    #endregion

    #region Custom Data

    /// <summary>
    /// Override this method in a subclass to load custom
    /// data beyond the automatically loaded values from
    /// the membership and role providers.
    /// </summary>
    protected internal virtual void LoadCustomData() { }

    #endregion

    #region Criteria

    /// <summary>
    /// Criteria object containing the user credentials
    /// to be authenticated.
    /// </summary>
    [Serializable()]
    public class Criteria : Csla.Core.MobileObject
    {
      private Criteria() { }
      /// <summary>
      /// Gets or sets the username.
      /// </summary>
      public string Name { get; set; }
      /// <summary>
      /// Gets or sets the password.
      /// </summary>
      public string Password { get; set; }
      /// <summary>
      /// Gets or sets the membership identity type.
      /// </summary>
      public string MembershipIdentityType { get; set; }
      /// <summary>
      /// Gets or sets whether the membership provider
      /// should be access on the client (true) or application
      /// server (false).
      /// </summary>
      public bool IsRunOnWebServer { get; set; }

      /// <summary>
      /// Creates an instance of the class.
      /// </summary>
      /// <param name="name">Username.</param>
      /// <param name="password">Password.</param>
      /// <param name="membershipIdentityType">Membership identity type.</param>
      /// <param name="isRunOnWebServer">
      /// Access membership provider locally (true) or via the data portal
      /// on an application server (false).
      /// </param>
      public Criteria(string name, string password, Type membershipIdentityType, bool isRunOnWebServer)
      {
        Name = name;
        Password = password;
        MembershipIdentityType = membershipIdentityType.AssemblyQualifiedName;
        IsRunOnWebServer = isRunOnWebServer;
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
        info.AddValue("MembershipIdentity.Criteria.IsRunOnWebServer", IsRunOnWebServer);
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
        IsRunOnWebServer = info.GetValue<bool>("MembershipIdentity.Criteria.IsRunOnWebServer");
      }
    }
    #endregion

  }
}
