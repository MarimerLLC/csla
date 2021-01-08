//-----------------------------------------------------------------------
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
      return new CslaDataConfiguration();
    }
  }

  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data portal.
  /// </summary>
  public class CslaDataConfiguration
  {
    /// <summary>
    /// Sets the default transaction isolation level.
    /// </summary>
    /// <param name="level">The default transaction isolation level</param>
    public CslaDataConfiguration DefaultTransactionIsolationLevel(TransactionIsolationLevel level)
    {
      ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"] = level.ToString();
      return this;
    }

    /// <summary>
    /// Sets the default transaction timeout in seconds.
    /// </summary>
    /// <param name="seconds">The default transaction timeout in seconds</param>
    public CslaDataConfiguration DefaultTransactionTimeoutInSeconds(int seconds)
    {
      ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"] = seconds.ToString();
      return this;
    }

    /// <summary>
    /// Sets the default transaction async flow option
    /// used to create new TransactionScope objects.
    /// </summary>
    /// <param name="asyncFlowOption">Async flow option</param>
    public CslaDataConfiguration DefaultTransactionAsyncFlowOption(System.Transactions.TransactionScopeAsyncFlowOption asyncFlowOption)
    {
      ConfigurationManager.AppSettings["CslaDefaultTransactionAsyncFlowOption"] = asyncFlowOption.ToString();
      return this;
    }
  }
}
