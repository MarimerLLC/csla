using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable()]
  public class CredentialsCriteria : CriteriaBase
  {

    public CredentialsCriteria() { }

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

    public CredentialsCriteria(string username, string password, string roles)
      : base(typeof(CredentialsCriteria))
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
}
