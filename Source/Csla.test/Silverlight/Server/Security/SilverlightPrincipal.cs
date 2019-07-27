//-----------------------------------------------------------------------
// <copyright file="SilverlightPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;

namespace Csla.Testing.Business.Security
{
  [Serializable]
  public partial class SilverlightPrincipal : Csla.Security.CslaPrincipal
  {
    public const string VALID_TEST_UID = "user";
    public const string VALID_TEST_PWD = "1234";

    public SilverlightPrincipal() { }
    private SilverlightPrincipal(IIdentity identity) : base(identity) { }
    private static bool SetPrincipal(IIdentity identity)
    {
      SilverlightPrincipal principal = new SilverlightPrincipal(identity);
      Csla.ApplicationContext.User = principal;
      return identity.IsAuthenticated;
    }

    public override bool IsInRole(string role)
    {
      var check = Identity as Csla.Security.ICheckRoles;
      
      return check != null && check.IsInRole(role);
    }

    [Serializable]
    public class Criteria : Core.MobileObject
    {
      private Criteria() { }
      public string Name { get; set; }
      public string Password { get; set; }
      public string ProviderType { get; set; }
      public Criteria(string name, string password)
      {
        Name = name;
        Password = password;
        ProviderType = string.Empty;
      }
      public Criteria(string name, string password, string providerType)
      {
        Name = name;
        Password = password;
        ProviderType = providerType;
      }

      protected override void OnGetState(Serialization.Mobile.SerializationInfo info, Core.StateMode mode)
      {
        info.AddValue("SilverlightPrincipal.Criteria.Name", Name);
        info.AddValue("SilverlightPrincipal.Criteria.Password", Password);
        info.AddValue("SilverlightPrincipal.Criteria.ProviderType", ProviderType);
        base.OnGetState(info, mode);
      }
      protected override void OnSetState(Serialization.Mobile.SerializationInfo info, Core.StateMode mode)
      {
        base.OnSetState(info, mode);
        Name = info.GetValue<string>("SilverlightPrincipal.Criteria.Name");
        Password = info.GetValue<string>("SilverlightPrincipal.Criteria.Password");
        ProviderType = info.GetValue<string>("SilverlightPrincipal.Criteria.ProviderType");
      }
    }
  }
}