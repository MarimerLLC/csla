using System;

namespace CSLA
{
  /// <summary>
  /// Provides information about the DataPortal 
  /// call.
  /// </summary>
  public class DataPortalEventArgs : EventArgs
  {
    /// <summary>
    /// The DataPortalContext object passed to the
    /// server-side DataPortal.
    /// </summary>
    public Server.DataPortalContext DataPortalContext;

    public DataPortalEventArgs(Server.DataPortalContext dataPortalContext)
    {
      DataPortalContext = dataPortalContext;
    }
  }
}
