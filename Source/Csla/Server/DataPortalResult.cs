//-----------------------------------------------------------------------
// <copyright file="DataPortalResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Returns data from the server-side DataPortal to the </summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;

namespace Csla.Server
{
  /// <summary>
  /// Returns data from the server-side DataPortal to the 
  /// client-side DataPortal. Intended for internal CSLA .NET
  /// use only.
  /// </summary>
  [Serializable]
  public class DataPortalResult : EventArgs, Core.IUseApplicationContext
  {
    internal ApplicationContext ApplicationContext { get; set; }
    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }

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
    /// Creates an instance of the type.
    /// </summary>
    public DataPortalResult()
    {
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    public DataPortalResult(ApplicationContext applicationContext, object returnObject)
    {
      ApplicationContext = applicationContext;
      ReturnObject = returnObject;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    /// <param name="ex">
    /// Error that occurred during the DataPotal call.
    /// This will be null if no errors occurred.
    /// </param>
    public DataPortalResult(ApplicationContext applicationContext, object returnObject, Exception ex)
    {
      ReturnObject = returnObject;
      Error = ex;
    }
  }
}