//-----------------------------------------------------------------------
// <copyright file="MockPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;
using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using System;

namespace DataBinding.Business
{
  [Serializable]
  public class MockPrincipal : MobileObject, IPrincipal
  {
    private MockIdentity _identity;

#if SILVERLIGHT
    public MockPrincipal() { }
#else
    private MockPrincipal() { }
#endif

    private MockPrincipal(string name)
    {
      switch (name)
      {
        case "admin":
          _identity = new MockIdentity(name, true, "Administrators");
          break;
        case "user":
          _identity = new MockIdentity(name, true, "Users");
          break;
        case "guest":
          _identity = new MockIdentity(name, true, "Guests");
          break;
        default:
          _identity = new MockIdentity(name, false);
          break;
      }
    }

    public static void Login(string name)
    {
      MockPrincipal principal = new MockPrincipal(name);
      Csla.ApplicationContext.User = principal;
    }

    public static void Logout()
    {
      Csla.ApplicationContext.User = null;
    }

    #region IPrincipal Members

    public IIdentity Identity
    {
      get { return _identity; }
    }

    public bool IsInRole(string role)
    {
      return _identity.IsInRole(role);
    }

    #endregion

    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      SerializationInfo i = formatter.SerializeObject(_identity);
      info.AddChild("_identity", i.ReferenceId);
      base.OnGetChildren(info, formatter);
    }

    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      int refId = info.Children["_identity"].ReferenceId;
      _identity = (MockIdentity)formatter.GetObject(refId);
      base.OnSetChildren(info, formatter);
    }
  }
}