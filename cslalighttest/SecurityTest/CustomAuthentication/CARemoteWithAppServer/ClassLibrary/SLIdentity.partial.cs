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
      EndpointAddress endpoint = new EndpointAddress("http://localhost:4769/Authentication.svc");

      ClassLibrary.AuthenticationService.AuthenticationClient client = 
        new ClassLibrary.AuthenticationService.AuthenticationClient(bind, endpoint);
      string response = client.Authenticate(criteria.Username, criteria.Password);

      string[] results = response.Split(';');

      if (Boolean.Parse(results[0]))
      {
        SetCslaIdentity(new MobileList<string>(results[1].Split(',')), true, criteria.Username);
      }
      else
      {
        SetCslaIdentity(null, false, "");
      }
    }
  }
}
