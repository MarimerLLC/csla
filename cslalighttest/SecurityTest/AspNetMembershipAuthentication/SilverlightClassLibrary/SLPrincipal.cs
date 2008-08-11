using System;
using System.Security.Principal;
using Csla;
using Csla.Security;
using Csla.Serialization;

namespace SilverlightClassLibrary
{
  [Serializable]
  public class SLPrincipal : Csla.Security.BusinessPrincipalBase
  {
    #region Constructor

    public SLPrincipal(IIdentity identity)
      : base(identity)
    { }

    #endregion


    #region Login/Logout

    #if SILVERLIGHT
    public static void Login(string username, string password, string roles, EventHandler<EventArgs> completed)
    {
      MembershipIdentity.GetMembershipIdentity<MembershipIdentityStub>((o, e) =>
      {
       var slIdentity = e.Object;
       var result = SetPrincipal(slIdentity);

       slIdentity.SetRoles(roles);
       completed(null, new LoginEventArgs(result));

      }, username, password, true);
    }

    private static bool SetPrincipal(IIdentity identity)
    {
      if (identity == null)
        return false;

      ApplicationContext.User =
        identity.IsAuthenticated ?
          new SLPrincipal(identity) :
          new SLPrincipal(new MembershipIdentityStub());

      return identity.IsAuthenticated;
    }

    #endif
    public static void Logout()
    {
      ApplicationContext.User = new SLPrincipal(new MembershipIdentityStub());
    }

    #endregion

    #region LoginEventArgs

    public class LoginEventArgs : EventArgs
    {

      private bool _loginSucceded;

      public bool LoginSucceded
      {
        get
        {
          return _loginSucceded;
        }
      }

      public LoginEventArgs(bool loginSucceded)
      {
        _loginSucceded = loginSucceded;
      }
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
