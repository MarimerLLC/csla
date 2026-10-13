//-----------------------------------------------------------------------
// <copyright file="DataOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------

using System.Transactions;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data subsystem.
  /// </summary>
  public class DataOptions
  {
    /// <summary>
    /// Gets or sets the default transaction isolation level,
    /// used when a <see cref="TransactionalAttribute"/> does
    /// not specify an isolation level.
    /// </summary>
    public TransactionIsolationLevel DefaultTransactionIsolationLevel { get; set; } = TransactionIsolationLevel.Unspecified;

    /// <summary>
    /// Gets or sets the default transaction timeout in seconds,
    /// used when a <see cref="TransactionalAttribute"/> does
    /// not specify a timeout.
    /// </summary>
    public int DefaultTransactionTimeoutInSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the default transaction async flow option
    /// used to create new TransactionScope objects when a
    /// <see cref="TransactionalAttribute"/> does not specify one.
    /// </summary>
    public TransactionScopeAsyncFlowOption DefaultTransactionAsyncFlowOption { get; set; } = TransactionScopeAsyncFlowOption.Suppress;
  }
}
