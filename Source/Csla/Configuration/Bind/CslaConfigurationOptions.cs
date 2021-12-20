//-----------------------------------------------------------------------
// <copyright file="CslaConfigurationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains configuration options which can be loaded </summary>
//-----------------------------------------------------------------------
using System;
using System.Transactions;

namespace Csla.Configuration
{
  /// <summary>
  /// Contains configuration options which can be loaded 
  /// using dot net core configuration subsystem
  /// </summary>
  public class CslaConfigurationOptions
  {
    /// <summary>
    /// Gets or sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    public ApplicationContext.PropertyChangedModes PropertyChangedMode
    {
      get => ApplicationContext.PropertyChangedMode; set => ApplicationContext.PropertyChangedMode = value;
    }

    /// <summary>
    /// Gets or sets a value representing the application version
    /// for use in server-side data portal routing.
    /// </summary>
    /// <remarks>
    /// Application version used to create data portal
    /// routing tag (can not contain '-').
    /// If this value is set then you must use the
    /// .NET Core server-side Http data portal endpoint
    /// as a router so the request can be routed to
    /// another app server that is running the correct
    /// version of the application's assemblies.
    /// </remarks>
    public string VersionRoutingTag
    {
      get { return ConfigurationManager.AppSettings["CslaVersionRoutingTag"]; }
      set
      {
        if (!string.IsNullOrWhiteSpace(value))
          if (value.Contains("-") || value.Contains("/"))
            throw new ArgumentException("valueRoutingToken");
        ConfigurationManager.AppSettings["CslaVersionRoutingTag"] = value;
        ApplicationContext.VersionRoutingTag = null;
      }
    }

    /// <summary>
    /// Gets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    public Type SerializationFormatter { get => ApplicationContext.SerializationFormatter; set => ApplicationContext.SerializationFormatter = value; }

    /// <summary>
    /// Sets the default transaction isolation level.
    /// </summary>
    public TransactionIsolationLevel DefaultTransactionIsolationLevel { get => ApplicationContext.DefaultTransactionIsolationLevel; set => ApplicationContext.DefaultTransactionIsolationLevel = value; }

    /// <summary>
    /// Gets or sets the default transaction timeout in seconds.
    /// </summary>
    /// <value>
    /// The default transaction timeout in seconds.
    /// </value>
    public int DefaultTransactionTimeoutInSeconds { get => ApplicationContext.DefaultTransactionTimeoutInSeconds; set => ApplicationContext.DefaultTransactionTimeoutInSeconds = value; }

    /// <summary>
    /// Gets or sets the default transaction async flow option
    /// used to create new TransactionScope objects. (Enabled or Suppress)
    /// </summary>
    public TransactionScopeAsyncFlowOption DefaultTransactionAsyncFlowOption { get => ApplicationContext.DefaultTransactionAsyncFlowOption; set => ApplicationContext.DefaultTransactionAsyncFlowOption = value; }

    /// <summary>
    /// Gets or sets the data portal configuration options
    /// </summary>
    public CslaDataPortalConfigurationOptions DataPortal { get; set; }
  }
}