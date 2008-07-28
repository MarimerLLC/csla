using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using System.ServiceModel;

namespace ClassLibrary
{
  public partial class SLIdentity
  {
    public void DataPortal_Fetch(ClassLibrary.SLIdentity.CredentialsCriteria criteria)
    {
      BasicHttpBinding bind = new BasicHttpBinding();
      EndpointAddress endpoint = new EndpointAddress("http://localhost:3833/Authentication.svc");

      ClassLibrary.AuthenticationService.AuthenticationClient client = 
        new ClassLibrary.AuthenticationService.AuthenticationClient(bind, endpoint);
      bool response = client.Authenticate(criteria.Username, criteria.Password);

      if (response)
      {
        SetCslaIdentity(new MobileList<string>(criteria.Roles.Split(';')), true, criteria.Username);
      }
      else
      {
        SetCslaIdentity(null, false, "");
      }
    }
  }
}
