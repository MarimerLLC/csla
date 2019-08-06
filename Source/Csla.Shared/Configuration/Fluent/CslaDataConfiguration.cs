﻿//-----------------------------------------------------------------------
// <copyright file="CslaDataConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaDataConfiguration
  /// </summary>
  public static class CslaDataConfigurationExtension
  {
    /// <summary>
    /// Extension method for CslaDataConfiguration
    /// </summary>
    public static CslaDataConfiguration Data(this ICslaConfiguration config)
    {
      return new CslaDataConfiguration(config);
    }
  }

  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data portal.
  /// </summary>
  public class CslaDataConfiguration
  {
    private ICslaConfiguration RootConfiguration { get; set; }

    internal CslaDataConfiguration(ICslaConfiguration root)
    {
      RootConfiguration = root;
    }

    /// <summary>
    /// Sets the default transaction isolation level.
    /// </summary>
    /// <param name="level">The default transaction isolation level</param>
    public ICslaConfiguration DefaultTransactionIsolationLevel(TransactionIsolationLevel level)
    {
      ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"] = level.ToString();
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the default transaction timeout in seconds.
    /// </summary>
    /// <param name="seconds">The default transaction timeout in seconds</param>
    public ICslaConfiguration DefaultTransactionTimeoutInSeconds(int seconds)
    {
      ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"] = seconds.ToString();
      return RootConfiguration;
    }

#if !NET40 && !NET45
    /// <summary>
    /// Sets the default transaction async flow option
    /// used to create new TransactionScope objects.
    /// </summary>
    /// <param name="asyncFlowOption">Async flow option</param>
    public ICslaConfiguration DefaultTransactionAsyncFlowOption(System.Transactions.TransactionScopeAsyncFlowOption asyncFlowOption)
    {
      ConfigurationManager.AppSettings["CslaDefaultTransactionAsyncFlowOption"] = asyncFlowOption.ToString();
      return RootConfiguration;
    }
#endif
  }
}
