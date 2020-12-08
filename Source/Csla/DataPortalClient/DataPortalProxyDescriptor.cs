using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Describes the data portal proxy for use by
  /// a specific business object type.
  /// </summary>
  public class DataPortalProxyDescriptor
  {
    /// <summary>
    /// Assembly qualified type name of the proxy
    /// </summary>
    public string ProxyTypeName { get; set; }
    /// <summary>
    /// Server endpoint URL to be called by the proxy
    /// </summary>
    public string DataPortalUrl { get; set; }
  }
}
