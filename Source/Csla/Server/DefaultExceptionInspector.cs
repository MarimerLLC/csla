//-----------------------------------------------------------------------
// <copyright file="DefaultExceptionInspector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default implementation of exception inspector.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Server
{
  /// <summary>
  /// Default implementation of exception inspector.
  /// </summary>
  internal class DefaultExceptionInspector : IDataPortalExceptionInspector
  {
    /// <summary>
    /// Inspects the exception that occurred during DataPortal call
    /// If you want to transform to/return another Exception to the client
    /// you must throw the new Exception in this method.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="businessObject">The business object , if available.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="ex">The exception.</param>
    public void InspectException(Type objectType, object businessObject, object criteria, string methodName, Exception ex)
    { }
  }
}
