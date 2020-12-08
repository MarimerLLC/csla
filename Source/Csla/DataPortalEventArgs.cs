//-----------------------------------------------------------------------
// <copyright file="DataPortalEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides information about the DataPortal </summary>
//-----------------------------------------------------------------------
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
    private Type _objectType;
    private object _object;

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
    /// Gets the object type being processed by the 
    /// data portal.
    /// </summary>
    public Type ObjectType
    {
      get { return _objectType; }
    }

    /// <summary>
    /// Gets the criteria object or business object
    /// being processed by the data portal.
    /// </summary>
    public object Object
    {
      get { return _object; }
    }

    /// <summary>
    /// Creates an instance of the object.
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
    public DataPortalEventArgs(Server.DataPortalContext dataPortalContext, Type objectType, object obj, DataPortalOperations operation)
    {
      _dataPortalContext = dataPortalContext;
      _operation = operation;
      _objectType = objectType;
      _object = obj;
    }

    /// <summary>
    /// Creates an instance of the object.
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
    public DataPortalEventArgs(Server.DataPortalContext dataPortalContext, Type objectType, object obj, DataPortalOperations operation, Exception exception)
      : this(dataPortalContext, objectType, obj, operation)
    {
      _exception = exception;
    }
  }
}