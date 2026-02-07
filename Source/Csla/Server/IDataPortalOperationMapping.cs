//-----------------------------------------------------------------------
// <copyright file="IDataPortalOperationMapping.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface for source-generated data portal operation dispatch</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Implemented by source-generated code to provide explicit
  /// data portal operation method invocation without reflection.
  /// </summary>
  public interface IDataPortalOperationMapping
  {
    /// <summary>
    /// Invoke a data portal operation method.
    /// </summary>
    /// <param name="operationType">The type of operation attribute (e.g. typeof(CreateAttribute))</param>
    /// <param name="isSync">Whether the client is calling synchronously</param>
    /// <param name="criteria">The criteria parameters, or null/empty for no-criteria operations</param>
    /// <param name="serviceProvider">Service provider for resolving injected dependencies</param>
    /// <returns>A Task representing the async operation</returns>
    /// <exception cref="DataPortalOperationNotSupportedException">
    /// Thrown when the operation/criteria combination is not handled by the generated code
    /// </exception>
    Task InvokeOperationAsync(Type operationType, bool isSync, object?[]? criteria, IServiceProvider serviceProvider);
  }
}
