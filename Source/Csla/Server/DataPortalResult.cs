//-----------------------------------------------------------------------
// <copyright file="DataPortalResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Returns data from the server-side DataPortal to the </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Specialized;
using Csla.Core;

namespace Csla.Server
{
  /// <summary>
  /// Returns data from the server-side DataPortal to the 
  /// client-side DataPortal. Intended for internal CSLA .NET
  /// use only.
  /// </summary>
  [Serializable]
  public class DataPortalResult : EventArgs
  {
    /// <summary>
    /// The business object being returned from
    /// the server.
    /// </summary>
    public object ReturnObject { get; private set; }

    /// <summary>
    /// Error that occurred during the DataPotal call.
    /// This will be null if no errors occurred.
    /// </summary>
    public Exception Error { get; private set; }

    /// <summary>
    /// The global context being returned from
    /// the server.
    /// </summary>
    public ContextDictionary GlobalContext { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public DataPortalResult()
    {
      GlobalContext = ApplicationContext.ContextManager.GetGlobalContext();
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    public DataPortalResult(object returnObject)
    {
      ReturnObject = returnObject;
      GlobalContext = ApplicationContext.ContextManager.GetGlobalContext();
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    /// <param name="globalContext">  Global context delivered via current reuest from the server
    /// </param>
    public DataPortalResult(object returnObject, ContextDictionary globalContext)
    {
      ReturnObject = returnObject;
      GlobalContext = globalContext;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    /// <param name="ex">
    /// Error that occurred during the DataPotal call.
    /// This will be null if no errors occurred.
    /// </param>
    /// <param name="globalContext">  Global context delivered via current reuest from the server
    /// </param>
    public DataPortalResult(object returnObject, Exception ex, ContextDictionary globalContext)
    {
      ReturnObject = returnObject;
      Error = ex;
      GlobalContext = globalContext;
    }
  }
}