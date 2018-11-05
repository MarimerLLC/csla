using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data portal.
  /// </summary>
  public class CslaDataConfiguration
  {
    private CslaConfiguration RootConfiguration { get; set; }

    internal CslaDataConfiguration(CslaConfiguration root)
    {
      RootConfiguration = root;
    }

    /// <summary>
    /// Sets the default transaction isolation level.
    /// </summary>
    /// <param name="level">The default transaction isolation level</param>
    public CslaConfiguration DefaultTransactionIsolationLevel(TransactionIsolationLevel level)
    {
      ApplicationContext.DefaultTransactionIsolationLevel = level;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the default transaction timeout in seconds.
    /// </summary>
    /// <param name="seconds">The default transaction timeout in seconds</param>
    public CslaConfiguration DefaultTransactionTimeoutInSeconds(int seconds)
    {
      ApplicationContext.DefaultTransactionTimeoutInSeconds = seconds;
      return RootConfiguration;
    }
  }
}
