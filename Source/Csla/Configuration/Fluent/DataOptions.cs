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

    /// <summary>
    /// Sets the default transaction async flow option
    /// used to create new TransactionScope objects.
    /// </summary>
    public TransactionScopeAsyncFlowOption DefaultTransactionAsyncFlowOption {
      get => defaultTransactionScopeAsyncFlowOption;
      set => defaultTransactionScopeAsyncFlowOption = value;
    }

    private static TransactionIsolationLevel defaultTransactionIsolationLevel = TransactionIsolationLevel.Unspecified;
    private static int defaultTransactionTimeoutInSeconds = 30;
    private static TransactionScopeAsyncFlowOption defaultTransactionScopeAsyncFlowOption = TransactionScopeAsyncFlowOption.Suppress;

    /// <summary>
    /// Gets the default transaction isolation level.
    /// </summary>
    /// <value>
    /// The default transaction isolation level.
    /// </value>
    internal static TransactionIsolationLevel GetDefaultTransactionIsolationLevel() => defaultTransactionIsolationLevel;

    /// <summary>
    /// Gets default transaction timeout in seconds.
    /// </summary>
    /// <value>
    /// The default transaction timeout in seconds.
    /// </value>
    internal static int GetDefaultTransactionTimeoutInSeconds() => defaultTransactionTimeoutInSeconds;

    /// <summary>
    /// Gets the default transaction scope async flow option.
    /// </summary>
    /// <returns>The default transaction scope async flow option.</returns>
    internal static TransactionScopeAsyncFlowOption GetDefaultTransactionScopeAsyncFlowOption() => defaultTransactionScopeAsyncFlowOption;

#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
    /// <summary>
    /// Sets the invariant name of a provider for
    /// use by DbProviderFactories.GetFactory().
    /// </summary>
    /// <param name="dbProvider"></param>
    [Obsolete("Use dependency injection", true)]
    public DataOptions DbProvider(string dbProvider)
    {
      Data.ConnectionManager.DbProvider = dbProvider;
      return this;
    }
#endif
  }
}
