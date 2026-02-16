//-----------------------------------------------------------------------
// <copyright file="IDataPortalOperationNamedMapping.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface for name-based data portal operation dispatch</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Implemented by source-generated code to provide name-based
  /// data portal operation method invocation without reflection.
  /// </summary>
  public interface IDataPortalOperationNamedMapping
  {
    /// <summary>
    /// Invoke a data portal operation method by name.
    /// </summary>
    /// <param name="operationName">The deterministic operation name (e.g. "Fetch__Int32")</param>
    /// <param name="isSync">Whether the client is calling synchronously</param>
    /// <param name="criteria">The criteria parameters, or null/empty for no-criteria operations</param>
    /// <param name="serviceProvider">Service provider for resolving injected dependencies</param>
    /// <returns>A Task representing the async operation</returns>
    /// <exception cref="DataPortalOperationNotSupportedException">
    /// Thrown when the operation name is not handled by the generated code
    /// </exception>
    Task InvokeNamedOperationAsync(string operationName, bool isSync, object?[]? criteria, IServiceProvider serviceProvider);
  }
}
