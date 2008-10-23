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
  [Serializable()]
  [MobileFactory("Csla.Security.IdentityFactory,Csla", "FetchMembershipIdentity")]
  public partial class MembershipIdentity : ReadOnlyBase<MembershipIdentity>, IIdentity, ICheckRoles
  {
    #region Constructor, Helper Setter

    private static int _forceInit;

    protected MembershipIdentity() 
    { 
      _forceInit = 0;
    }

    #endregion

    #region  IsInRole

    private static readonly PropertyInfo<MobileList<string>> RolesProperty =
      RegisterProperty(new PropertyInfo<MobileList<string>>("Roles"));
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

    protected internal virtual void LoadCustomData() { }

    #endregion

    #region Criteria
    [Serializable()]
    public class Criteria : Csla.Core.MobileObject
    {
      private Criteria() { }
      public string Name { get; set; }
      public string Password { get; set; }
      public string MembershipIdentityType { get; set; }
      public bool IsRunOnWebServer { get; set; }
      public Criteria(string name, string password, Type membershipIdentityType, bool isRunOnWebServer)
      {
        Name = name;
        Password = password;
        MembershipIdentityType = membershipIdentityType.AssemblyQualifiedName;
        IsRunOnWebServer = isRunOnWebServer;
      }
      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        info.AddValue("MembershipIdentity.Criteria.Name", Name);
        info.AddValue("MembershipIdentity.Criteria.Password", Password);
        info.AddValue("MembershipIdentity.Criteria.MembershipIdentityType", MembershipIdentityType);
        info.AddValue("MembershipIdentity.Criteria.IsRunOnWebServer", IsRunOnWebServer);
        base.OnGetState(info, mode);
      }
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
