//-----------------------------------------------------------------------
// <copyright file="DataPortalResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>DataPortalResult defines the results of DataPortal operation.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// DataPortalResult defines the results of DataPortal operation.
  /// It contains object that was received from the server, 
  /// an error (if occurred) and userState - user defined information
  /// that was passed into data portal on initial request
  /// </summary>
  /// <typeparam name="T">Type of object that DataPortal received</typeparam>
  public class DataPortalResult<T> : EventArgs, IDataPortalResult
  {
    /// <summary>
    /// Object that DataPortal received as a result of current operation
    /// </summary>
    public T Object { get; private set; }
    /// <summary>
    /// Error that occurred during the DataPotal call.
    /// This will be null if no errors occurred.
    /// </summary>
    public Exception Error { get; private set; }

    /// <summary>
    /// User defined information
    /// that was passed into data portal on initial request
    /// </summary>
    public object UserState { get; private set; }

    /// <summary>
    /// Create new instance of data portal result
    /// </summary>
    /// <param name="obj">
    /// Object that DataPortal received as a result of current operation
    /// </param>
    /// <param name="ex">
    /// Error that occurred during the DataPotal call.
    /// This will be null if no errors occurred.
    /// </param>
    /// <param name="userState">
    /// User defined information
    /// that was passed into data portal on initial request
    /// </param>
    public DataPortalResult(T obj, Exception ex, object userState)
    {
      this.Object = obj;
      this.Error = ex;
      this.UserState = userState;
    }

    object IDataPortalResult.Object
    {
      get { return this.Object; }
    }

    Exception IDataPortalResult.Error
    {
      get { return this.Error; }
    }

    object IDataPortalResult.UserState
    {
      get { return this.UserState; }
    }
  }
}