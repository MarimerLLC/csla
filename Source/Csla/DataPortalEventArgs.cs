//-----------------------------------------------------------------------
// <copyright file="DataPortalEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides information about the DataPortal </summary>
//-----------------------------------------------------------------------

namespace Csla
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
    public Server.DataPortalContext? DataPortalContext { get; }

    /// <summary>
    /// Gets the requested data portal operation.
    /// </summary>
    public DataPortalOperations Operation { get; }

    /// <summary>
    /// Gets a reference to any exception that occurred
    /// during the data portal call.
    /// </summary>
    /// <remarks>
    /// This property will return Nothing (null in C#) if no
    /// exception occurred. Exceptions are returned only as part
    /// of a data portal complete event or method.
    /// </remarks>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the object type being processed by the 
    /// data portal.
    /// </summary>
    public Type ObjectType { get; }

    /// <summary>
    /// Gets the criteria object or business object
    /// being processed by the data portal.
    /// </summary>
    public object? Object { get; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="dataPortalContext">
    /// Data portal context object.
    /// </param>
    /// <param name="objectType">
    /// Business object type.
    /// </param>
    /// <param name="obj">
    /// Criteria or business object for request.
    /// </param>
    /// <param name="operation">
    /// Data portal operation being performed.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public DataPortalEventArgs(Server.DataPortalContext? dataPortalContext, Type objectType, object? obj, DataPortalOperations operation)
    {
      DataPortalContext = dataPortalContext;
      Operation = operation;
      ObjectType = Guard.NotNull(objectType);
      Object = obj;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="dataPortalContext">
    /// Data portal context object.
    /// </param>
    /// <param name="objectType">
    /// Business object type.
    /// </param>
    /// <param name="obj">
    /// Criteria or business object for request.
    /// </param>
    /// <param name="operation">
    /// Data portal operation being performed.
    /// </param>
    /// <param name="exception">
    /// Exception encountered during processing.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="exception"/> is <see langword="null"/>.</exception>
    public DataPortalEventArgs(Server.DataPortalContext? dataPortalContext, Type objectType, object? obj, DataPortalOperations operation, Exception exception)
      : this(dataPortalContext, objectType, obj, operation)
    {
      Exception = Guard.NotNull(exception);
    }
  }
}