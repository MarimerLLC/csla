//-----------------------------------------------------------------------
// <copyright file="TransactionScopeFactoryContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Information passed to an ITransactionScopeFactory</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Information about a data portal operation, passed
  /// to an <see cref="ITransactionScopeFactory"/> so it
  /// can create the transaction scope for that operation.
  /// </summary>
  public sealed class TransactionScopeFactoryContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionScopeFactoryContext"/> class.
    /// </summary>
    /// <param name="objectType">The business object type.</param>
    /// <param name="operation">The data portal operation.</param>
    /// <param name="method">The method that implements the operation.</param>
    /// <param name="transactionalAttribute">The transactional attribute applied to the method.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="method"/> or <paramref name="transactionalAttribute"/> is <see langword="null"/>.</exception>
    public TransactionScopeFactoryContext(Type objectType, DataPortalOperations operation, System.Reflection.MethodInfo method, TransactionalAttribute transactionalAttribute)
    {
      ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
      Operation = operation;
      Method = method ?? throw new ArgumentNullException(nameof(method));
      TransactionalAttribute = transactionalAttribute ?? throw new ArgumentNullException(nameof(transactionalAttribute));
    }

    /// <summary>
    /// Gets the business object type.
    /// </summary>
    public Type ObjectType { get; }

    /// <summary>
    /// Gets the data portal operation.
    /// </summary>
    public DataPortalOperations Operation { get; }

    /// <summary>
    /// Gets the method that implements the operation. This
    /// may be a method of the business type or of an
    /// object factory type.
    /// </summary>
    public System.Reflection.MethodInfo Method { get; }

    /// <summary>
    /// Gets the name of the method that implements the operation.
    /// </summary>
    public string MethodName => Method.Name;

    /// <summary>
    /// Gets the transactional attribute applied to the method.
    /// </summary>
    public TransactionalAttribute TransactionalAttribute { get; }
  }
}
