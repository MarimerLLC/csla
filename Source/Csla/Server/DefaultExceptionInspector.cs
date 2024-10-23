//-----------------------------------------------------------------------
// <copyright file="DefaultExceptionInspector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default implementation of exception inspector.</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Default implementation of exception inspector.
  /// </summary>
  internal class DefaultExceptionInspector : IDataPortalExceptionInspector
  {
    /// <inheritdoc />
    public void InspectException(Type objectType, object? businessObject, object? criteria, string methodName, Exception ex)
    { }
  }
}