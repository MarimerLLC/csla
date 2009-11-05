using System;
using System.Collections.Generic;
using System.Text;
using System.EnterpriseServices;
using System.Runtime.InteropServices;

namespace EnterpriseServicesHost
{
  [EventTrackingEnabled(true)]
  [ComVisible(true)]
  public class EnterpriseServicesPortal :
    Csla.Server.Hosts.EnterpriseServicesPortal
  {
    // no code needed - implementation is in the base class
  }
}
