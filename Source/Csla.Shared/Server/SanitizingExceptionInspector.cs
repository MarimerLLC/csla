//-----------------------------------------------------------------------
// <copyright file="SanitizingExceptionInspector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side sanitizing exception inspector.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Server
{
  /// <summary>
  /// Sanitizing implementation of exception inspector, for hiding 
  /// sensitive information in exception details.
  /// </summary>
  /// <remarks>Only sanitizes exceptions from remote dataportals</remarks>
  public class SanitizingExceptionInspector : IDataPortalExceptionInspector
  {

    /// <summary>
    /// Throws a generic, exception lacking details for return to the client, thus
    /// protecting the remote data portal from leaking sensitive exception messages
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="businessObject">The business object, if available.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="ex">The exception.</param>
    public void InspectException(Type objectType, object businessObject, object criteria, string methodName, Exception ex)
    {
      // Shortcut if we are not running on the server-side of a remote data portal operation
      if (ApplicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server ||
        ApplicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server)
        return;

      // Sanitize in all remaining scenarios
      string identifier = Guid.NewGuid().ToString();
      throw new ServerException(identifier);
    }
  }
}