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
    private DataPortalOperations _operation;

    /// <summary>
    /// The DataPortalContext object passed to the
    /// server-side DataPortal.
    /// </summary>
    public Server.DataPortalContext DataPortalContext
    {
      get { return _dataPortalContext; }
    }

    /// <summary>
    /// Gets the requested data portal operation.
    /// </summary>
    public DataPortalOperations Operation
    {
      get { return _operation; }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="dataPortalContext">
    /// Data portal context object.
    /// </param>
    /// <param name="operation">
    /// Data portal operation being performed.
    /// </param>
    public DataPortalEventArgs(Server.DataPortalContext dataPortalContext, DataPortalOperations operation)
    {
      _dataPortalContext = dataPortalContext;
      _operation = operation;
    }
  }
}