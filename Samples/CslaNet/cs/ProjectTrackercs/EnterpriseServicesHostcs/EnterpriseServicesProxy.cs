using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServicesHost
{
  public class EnterpriseServicesProxy :
    Csla.DataPortalClient.EnterpriseServicesProxy
  {
    protected override 
      Csla.Server.Hosts.EnterpriseServicesPortal GetServerObject()
    {
      return new EnterpriseServicesPortal();
    }
  }
}
