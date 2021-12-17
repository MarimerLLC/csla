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
    /// <param name="level">The default transaction isolation level</param>
    public DataOptions DefaultTransactionIsolationLevel(TransactionIsolationLevel level)
    {
      ApplicationContext.DefaultTransactionIsolationLevel = level;
      return this;
    }

    /// <summary>
    /// Sets the default transaction timeout in seconds.
    /// </summary>
    /// <param name="seconds">The default transaction timeout in seconds</param>
    public DataOptions DefaultTransactionTimeoutInSeconds(int seconds)
    {
      ApplicationContext.DefaultTransactionTimeoutInSeconds = seconds;
      return this;
    }

    /// <summary>
    /// Sets the default transaction async flow option
    /// used to create new TransactionScope objects.
    /// </summary>
    /// <param name="asyncFlowOption">Async flow option</param>
    public DataOptions DefaultTransactionAsyncFlowOption(TransactionScopeAsyncFlowOption asyncFlowOption)
    {
      ApplicationContext.DefaultTransactionAsyncFlowOption = asyncFlowOption;
      return this;
    }

#if !NETSTANDARD2_0 && !NET5_0 && !NET6_0
    /// <summary>
    /// Sets the invariant name of a provider for
    /// use by DbProviderFactories.GetFactory().
    /// </summary>
    /// <param name="dbProvider"></param>
    /// <returns></returns>
    public DataOptions DbProvider(string dbProvider)
    {
      Data.ConnectionManager.DbProvider = dbProvider;
      return this;
    }
#endif
  }
}
