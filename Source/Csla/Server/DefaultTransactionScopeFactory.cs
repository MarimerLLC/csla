//-----------------------------------------------------------------------
// <copyright file="DefaultTransactionScopeFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default implementation of ITransactionScopeFactory</summary>
//-----------------------------------------------------------------------

using System.Runtime.Versioning;
using System.Transactions;
using Csla.Configuration;

namespace Csla.Server
{
  /// <summary>
  /// Default implementation of <see cref="ITransactionScopeFactory"/>.
  /// Uses the values from the <see cref="TransactionalAttribute"/>,
  /// falling back to the defaults in <see cref="DataOptions"/> for
  /// any value the attribute does not specify.
  /// </summary>
  [UnsupportedOSPlatform("browser")]
  public class DefaultTransactionScopeFactory : ITransactionScopeFactory
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTransactionScopeFactory"/> class.
    /// </summary>
    /// <param name="options">The CSLA options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    public DefaultTransactionScopeFactory(CslaOptions options)
    {
      if (options is null)
        throw new ArgumentNullException(nameof(options));
      DataOptions = options.DataOptions;
    }

    /// <summary>
    /// Gets the data options that provide default transaction values.
    /// </summary>
    protected DataOptions DataOptions { get; }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    public virtual TransactionScope CreateTransactionScope(TransactionScopeFactoryContext context)
    {
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      return new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions(context), GetAsyncFlowOption(context));
    }

    /// <summary>
    /// Gets the transaction options (isolation level and timeout)
    /// for the operation.
    /// </summary>
    /// <param name="context">Information about the data portal operation.</param>
    protected virtual TransactionOptions GetTransactionOptions(TransactionScopeFactoryContext context)
    {
      var attribute = context.TransactionalAttribute;
      var isolationLevel = attribute.TransactionIsolationLevel;
      if (isolationLevel == TransactionIsolationLevel.Unspecified)
        isolationLevel = DataOptions.DefaultTransactionIsolationLevel;

      return new TransactionOptions
      {
        IsolationLevel = GetIsolationLevel(isolationLevel),
        Timeout = TimeSpan.FromSeconds(attribute.TimeoutInSeconds ?? DataOptions.DefaultTransactionTimeoutInSeconds)
      };
    }

    /// <summary>
    /// Gets the async flow option for the operation.
    /// </summary>
    /// <param name="context">Information about the data portal operation.</param>
    protected virtual TransactionScopeAsyncFlowOption GetAsyncFlowOption(TransactionScopeFactoryContext context)
      => context.TransactionalAttribute.AsyncFlowOption ?? DataOptions.DefaultTransactionAsyncFlowOption;

    private static IsolationLevel GetIsolationLevel(TransactionIsolationLevel transactionIsolationLevel)
    {
      return transactionIsolationLevel switch
      {
        TransactionIsolationLevel.Serializable => IsolationLevel.Serializable,
        TransactionIsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
        TransactionIsolationLevel.ReadCommitted => IsolationLevel.ReadCommitted,
        TransactionIsolationLevel.ReadUncommitted => IsolationLevel.ReadUncommitted,
        TransactionIsolationLevel.Snapshot => IsolationLevel.Snapshot,
        _ => IsolationLevel.Unspecified
      };
    }
  }
}
