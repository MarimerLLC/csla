//-----------------------------------------------------------------------
// <copyright file="DataOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
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
    /// Sets the default transaction isolation level.
    /// </summary>
    public TransactionIsolationLevel DefaultTransactionIsolationLevel 
    {
      get => defaultTransactionIsolationLevel;
      set => defaultTransactionIsolationLevel = value; 
    }

    /// <summary>
    /// Sets the default transaction timeout in seconds.
    /// </summary>
    public int DefaultTransactionTimeoutInSeconds
    {
      get => defaultTransactionTimeoutInSeconds;
      set => defaultTransactionTimeoutInSeconds = value;
    }

    internal static TransactionIsolationLevel defaultTransactionIsolationLevel = TransactionIsolationLevel.Unspecified;
    internal static int defaultTransactionTimeoutInSeconds = 30;

    /// <summary>
    /// Gets or sets the default transaction isolation level.
    /// </summary>
    /// <value>
    /// The default transaction isolation level.
    /// </value>
    internal static TransactionIsolationLevel GetDefaultTransactionIsolationLevel() => defaultTransactionIsolationLevel;

    /// <summary>
    /// Gets or sets the default transaction timeout in seconds.
    /// </summary>
    /// <value>
    /// The default transaction timeout in seconds.
    /// </value>
    internal static int GetDefaultTransactionTimeoutInSeconds() => defaultTransactionTimeoutInSeconds;

    /// <summary>
    /// Sets the default transaction async flow option
    /// used to create new TransactionScope objects.
    /// </summary>
    public TransactionScopeAsyncFlowOption DefaultTransactionAsyncFlowOption { get; set; } = System.Transactions.TransactionScopeAsyncFlowOption.Suppress;

#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
    /// <summary>
    /// Sets the invariant name of a provider for
    /// use by DbProviderFactories.GetFactory().
    /// </summary>
    /// <param name="dbProvider"></param>
    /// <returns></returns>
    [Obsolete("Use dependency injection", false)]
    public DataOptions DbProvider(string dbProvider)
    {
      Data.ConnectionManager.DbProvider = dbProvider;
      return this;
    }
#endif
  }
}
