using System.Security.Principal;
using Csla.Core;
using Csla.Security;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using System;

namespace DataBinding.Business
{
  [Serializable]
  public class MockIdentity : MobileObject, IIdentity, ICheckRoles
  {
    private bool _isAuthenticated;
    private string _name;
    private MobileList<string> _roles = new MobileList<string>();
    
#if SILVERLIGHT
    public MockIdentity() { }
#else
    private MockIdentity() { }
#endif

    public MockIdentity(string name, bool isAuthenticated, params string[] roles)
    {
      _name = name;
      _isAuthenticated = isAuthenticated;
      _roles.AddRange(roles);
    }
    
    #region IIdentity Members

    public string AuthenticationType
    {
      get { return "mock"; }
    }

    public bool IsAuthenticated
    {
      get { return _isAuthenticated; }
    }

    public string Name
    {
      get { return _name; }
    }

    #endregion

    internal bool IsInRole(string role)
    {
      return _roles.Contains(role);
    }

    #region ICheckRoles Members

    bool ICheckRoles.IsInRole(string role)
    {
      return this.IsInRole(role);
    }

    #endregion

    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      int refId = info.Children["Roles"].ReferenceId;
      _roles = (MobileList<string>)formatter.GetObject(refId);
      base.OnSetChildren(info, formatter);
    }

    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _name = (string)info.Values["Name"].Value;
      _isAuthenticated = (bool)info.Values["IsAuthenticated"].Value;
      base.OnSetState(info, mode);
    }

    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      SerializationInfo i = formatter.SerializeObject(_roles);
      info.AddChild("Roles", i.ReferenceId);
      base.OnGetChildren(info, formatter);
    }
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("Name", Name);
      info.AddValue("IsAuthenticated", IsAuthenticated);
      base.OnGetState(info, mode);
    }
  }
}
