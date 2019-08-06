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
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.DataPortalClient;
using Csla.Core;

namespace SilverlightClassLibrary
{
  public class SLIdentity : Csla.Security.CslaIdentity
  {
    public bool InRole(string role)
    {
      return base.IsInRole(role);
    }

    [Serializable()]
    public class CredentialsCriteria : CriteriaBase
    {
      private string _username;
      private string _password;
      private string _roles;

      public string Username
      {
        get
        {
          return _username;
        }
      }

      public string Password
      {
        get
        {
          return _password;
        }
      }

      public string Roles
      {
        get
        {
          return _roles;
        }
      }

#if !SILVERLIGHT
      private CredentialsCriteria()
      { }
#endif

      public CredentialsCriteria(string username, string password, string roles)
        : base(typeof(SLIdentity))
      {
        _username = username;
        _password = password;
        _roles = roles;
      }

      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        info.AddValue("_username", _username);
        info.AddValue("_password", _password);
        info.AddValue("_roles", _roles);
        base.OnGetState(info, mode);
      }

      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        _username = (string)info.Values["_username"].Value;
        _password = (string)info.Values["_password"].Value;
        _roles = (string)info.Values["_roles"].Value;
        base.OnSetState(info, mode);
      }
    }

#if SILVERLIGHT
    public void DataPortal_Fetch(LocalProxy<SLIdentity>.CompletedHandler completed, CredentialsCriteria criteria)
    {
      if (criteria.Username == "TestUser" && criteria.Password == "1234")
      {
        SetCslaIdentity(new MobileList<string>(criteria.Roles.Split(';')), true, criteria.Username);
        completed(this, null);
      }
      else
      {
        SetCslaIdentity(null, false, "");
        completed(this, null);
      }
    }

    public static void GetIdentity(string username, string password, string roles, EventHandler<DataPortalResult<SLIdentity>> completed)
    {
      GetCslaIdentity<SLIdentity>(completed, new CredentialsCriteria(username, password, roles));
    }
#endif
  }
}
