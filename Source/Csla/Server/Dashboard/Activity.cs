using System;

namespace Csla.Server.Dashboard
{
  /// <summary>
  /// Information about a server-side data portal 
  /// invocation.
  /// </summary>
  public class Activity
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="result">InterceptArgs object from the data portal</param>
    public Activity(InterceptArgs result)
    {
      ObjectType = result.ObjectType;
      Operation = result.Operation;
      Runtime = result.Runtime;
      Exception = result.Exception;
    }

    /// <summary>
    /// Gets the root business object type for the call
    /// </summary>
    public Type ObjectType { get; private set; }
    /// <summary>
    /// Gets the operation type for the call
    /// </summary>
    public DataPortalOperations Operation { get; private set; }
    /// <summary>
    /// Gets the elapsed runtime for the call. Only
    /// valid upon call completion.
    /// </summary>
    public TimeSpan Runtime { get; private set; }
    /// <summary>
    /// Gets the exception (if any) resulting from the
    /// call. Only valid upon call completion.
    /// </summary>
    public Exception Exception { get; private set; }
  }
}
