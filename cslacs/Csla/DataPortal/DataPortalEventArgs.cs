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
    private Exception _exception;

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
    /// Gets a reference to any exception that occurred
    /// during the data portal call.
    /// </summary>
    /// <remarks>
    /// This property will return Nothing (null in C#) if no
    /// exception occurred. Exceptions are returned only as part
    /// of a data portal complete event or method.
    /// </remarks>
    public Exception Exception
    {
      get { return _exception; }
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

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="dataPortalContext">
    /// Data portal context object.
    /// </param>
    /// <param name="operation">
    /// Data portal operation being performed.
    /// </param>
    /// <param name="exception">
    /// Exception encountered during processing.
    /// </param>
    public DataPortalEventArgs(Server.DataPortalContext dataPortalContext, DataPortalOperations operation, Exception exception)
      : this(dataPortalContext, operation)
    {
      _exception = exception;
    }
  }
}