using System;
using System.Security.Principal;
using Csla;
using Csla.Security;
using Csla.Serialization;

namespace SilverlightClassLibrary
{
  [Serializable]
  public partial class SilverlightPrincipal : Csla.Security.BusinessPrincipalBase
  {
    #region Constructor
    public SilverlightPrincipal(){}
    public SilverlightPrincipal(IIdentity identity)
      : base(identity)
    { }

    #endregion


    #region Login/Logout

    private static bool SetPrincipal(IIdentity identity)
    {
      if (identity == null)
        return false;

      ApplicationContext.User =
        identity.IsAuthenticated ?
          new SilverlightPrincipal(identity) :
          new SilverlightPrincipal(new MembershipIdentityStub());

      return identity.IsAuthenticated;
    }
    public static void Logout()
    {
      ApplicationContext.User = new SilverlightPrincipal(new MembershipIdentityStub());
    }


    #endregion

    [Serializable]
    public class Criteria : Csla.Core.MobileObject
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

      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        info.AddValue("SilverlightPrincipal.Criteria.Name", Name);
        info.AddValue("SilverlightPrincipal.Criteria.Password", Password);
        info.AddValue("SilverlightPrincipal.Criteria.ProviderType", ProviderType);
        base.OnGetState(info, mode);
      }
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        base.OnSetState(info, mode);
        Name = info.GetValue<string>("SilverlightPrincipal.Criteria.Name");
        Password = info.GetValue<string>("SilverlightPrincipal.Criteria.Password");
        ProviderType = info.GetValue<string>("SilverlightPrincipal.Criteria.ProviderType");
      }
    }


  }
}
