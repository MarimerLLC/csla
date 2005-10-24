using System;

namespace Csla
{
  /// <summary>
  /// Provides information about the DataPortal 
  /// call.
  /// </summary>
  public class DataPortalEventArgs : EventArgs
  {
    private Server.DataPortalContext _dataPortalContext;

    /// <summary>
    /// The DataPortalContext object passed to the
    /// server-side DataPortal.
    /// </summary>
    public Server.DataPortalContext DataPortalContext
    {
      get { return _dataPortalContext; }
    }

    public DataPortalEventArgs(Server.DataPortalContext dataPortalContext)
    {
      _dataPortalContext = dataPortalContext;
    }
  }
}