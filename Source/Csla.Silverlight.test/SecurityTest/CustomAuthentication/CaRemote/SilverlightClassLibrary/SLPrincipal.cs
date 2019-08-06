using System;
using System.Net;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using System.Security.Principal;
using Csla.DataPortalClient;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary
{
  [Serializable()]
  public class SLPrincipal:Csla.Security.BusinessPrincipalBase
  {
    private SLPrincipal(IIdentity identity)
      : base(identity)
    { }

    public SLPrincipal(): base() {  }

#if SILVERLIGHT

    public static void Login(string username, string password, string roles, EventHandler<EventArgs> completed)
      {        
        SLIdentity.GetIdentity(username, password, roles,(o, e) =>
        {
          bool result = SetPrincipal(e.Object);
          completed(null, new LoginEventArgs(result));
        });
      }
#endif

    private static bool SetPrincipal(Csla.Security.CslaIdentity identity)
    {
      if (identity.IsAuthenticated)
      {
        SLPrincipal principal = new SLPrincipal(identity);
        Csla.ApplicationContext.User = principal;
      }
      else
      {
        identity = SLIdentity.UnauthenticatedIdentity();
        SLPrincipal principal = new SLPrincipal(identity);
        Csla.ApplicationContext.User = principal;
      }
      return identity.IsAuthenticated;
    }

    public static void Logout()
    {
      Csla.Security.CslaIdentity identity = SLIdentity.UnauthenticatedIdentity();
      SLPrincipal principal = new SLPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      return ((SLIdentity)base.Identity).InRole(role);
    }

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
  }
}
