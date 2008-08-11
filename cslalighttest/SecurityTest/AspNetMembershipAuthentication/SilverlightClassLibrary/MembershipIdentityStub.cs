using System;
using System.Net;
using Csla.DataPortalClient;
using Csla.Security;
using Csla.Serialization;
using Csla.Core;

namespace SilverlightClassLibrary
{
  [Serializable]
  public class MembershipIdentityStub : MembershipIdentity
  {
    public MembershipIdentityStub(){}

    protected override void LoadCustomData()
    {
      base.LoadCustomData();
    }

    public void SetRoles(string roles)
    {
      if (IsAuthenticated)
      {
        base.Roles = new MobileList<string>(roles.Split(';'));
      }
    }

    private void DataPortal_Fetch(object criteria)
    {
      if (((SLPrincipal.Criteria)criteria).Name == "invalidusername")
      {
        Roles = new MobileList<string>();
        IsAuthenticated = false;
        Name = string.Empty;
        AuthenticationType = "Csla";
      }
      else
      {
        Roles = new MobileList<string>(new string[] { "Admin", "User" });
        IsAuthenticated = true;
        Name = "SilverlightIdentity";
        AuthenticationType = "SilverLight";
      }
    }
    
    #if SILVERLIGHT
    public void DataPortal_Fetch(LocalProxy<MembershipIdentityStub>.CompletedHandler completed, object criteria)
    {
      Roles = new MobileList<string>(new string[] { "Admin", "User" });
      IsAuthenticated = true;
      Name = "SilverlightIdentity";
      AuthenticationType = "SilverLight";
      completed(this, null);
    } 
    #endif
  }
}
