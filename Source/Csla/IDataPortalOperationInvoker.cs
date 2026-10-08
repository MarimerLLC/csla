//-----------------------------------------------------------------------
// <copyright file="IDataPortalOperationInvoker.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Invokes pre-resolved data portal operations</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Invokes data portal operations whose target method has already been
  /// resolved at compile time. Used by generated data portal extension
  /// methods; not intended to be called directly.
  /// </summary>
  /// <remarks>
  /// The operation name identifies the operation method on the server
  /// and the <c>runLocal</c> flag reflects any
  /// <see cref="RunLocalAttribute"/> on that method, so the client does
  /// not need to locate the method using reflection.
  /// </remarks>
  /// <typeparam name="T">Type of business object.</typeparam>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IDataPortalOperationInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>
    where T : ICslaObject
  {
    /// <summary>
    /// Creates a business object using a pre-resolved create operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    T CreateByOperation(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Creates a business object using a pre-resolved create operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    Task<T> CreateByOperationAsync(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Fetches a business object using a pre-resolved fetch operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    T FetchByOperation(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Fetches a business object using a pre-resolved fetch operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    Task<T> FetchByOperationAsync(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Executes a command object using a pre-resolved execute operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    T ExecuteByOperation(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Executes a command object using a pre-resolved execute operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    Task<T> ExecuteByOperationAsync(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Deletes a business object using a pre-resolved delete operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    void DeleteByOperation(string operationName, bool runLocal, object?[]? criteria);

    /// <summary>
    /// Deletes a business object using a pre-resolved delete operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="runLocal">True to run the operation on the client.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    Task DeleteByOperationAsync(string operationName, bool runLocal, object?[]? criteria);
  }

  /// <summary>
  /// Invokes child data portal operations whose target method has already
  /// been resolved at compile time. Used by generated data portal extension
  /// methods; not intended to be called directly.
  /// </summary>
  /// <typeparam name="T">Type of business object.</typeparam>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IChildDataPortalOperationInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>
    where T : ICslaObject
  {
    /// <summary>
    /// Creates a child business object using a pre-resolved create child operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    T CreateChildByOperation(string operationName, object?[]? criteria);

    /// <summary>
    /// Creates a child business object using a pre-resolved create child operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    Task<T> CreateChildByOperationAsync(string operationName, object?[]? criteria);

    /// <summary>
    /// Fetches a child business object using a pre-resolved fetch child operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    T FetchChildByOperation(string operationName, object?[]? criteria);

    /// <summary>
    /// Fetches a child business object using a pre-resolved fetch child operation.
    /// </summary>
    /// <param name="operationName">Generated operation name.</param>
    /// <param name="criteria">Criteria values, one element per criteria parameter.</param>
    Task<T> FetchChildByOperationAsync(string operationName, object?[]? criteria);
  }
}
