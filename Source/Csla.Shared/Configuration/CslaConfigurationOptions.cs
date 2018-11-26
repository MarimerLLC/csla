//-----------------------------------------------------------------------
// <copyright file="CslaConfigurationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Contains configuration options which can be loaded </summary>
//-----------------------------------------------------------------------

using Csla.Security;
using Csla.Serialization.Mobile;
using System;
using static Csla.ApplicationContext;

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
    public string PropertyChangedMode
    {
      get { return ConfigurationManager.AppSettings["CslaPropertyChangedMode"]; }
      set { ConfigurationManager.AppSettings["CslaPropertyChangedMode"] = value; }
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
    public string VersionRoutingToken
    {
      get { return ConfigurationManager.AppSettings["CslaVersionRoutingToken"]; }
      set
      {
        if (!string.IsNullOrWhiteSpace(value))
          if (value.Contains("-") || value.Contains("/"))
            throw new ArgumentException("valueRoutingToken");
        ConfigurationManager.AppSettings["CslaVersionRoutingToken"] = value;
        ApplicationContext.VersionRoutingToken = null;
      }
    }

    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    public string PropertyInfoFactory
    {
      get { return ConfigurationManager.AppSettings["CslaPropertyInfoFactory"]; }
      set
      {        
        ConfigurationManager.AppSettings["CslaPropertyInfoFactory"] = value;
      }
    }

    /// <summary>
    /// Gets a qualified name for a method that implements
    /// the IsInRole() behavior used for authorization.
    /// </summary>
    /// <returns>
    /// Returns a value in the form
    /// "Namespace.Class, Assembly, MethodName".
    /// </returns>
    /// <remarks>
    /// The default is to use a simple IsInRole() call against
    /// the current principal. If another method is supplied
    /// it must conform to the IsInRoleProvider delegate.
    /// </remarks>
    public static string IsInRoleProvider
    {
      get { return ConfigurationManager.AppSettings["CslaIsInRoleProvider"]; }
      set { ConfigurationManager.AppSettings["CslaIsInRoleProvider"] = value; }
    }

    /// <summary>
    /// Gets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    public string SerializationFormatter
    {
      get { return ConfigurationManager.AppSettings["CslaSerializationFormatter"]; }
      set { ConfigurationManager.AppSettings["CslaSerializationFormatter"] = value; }
    }

    /// <summary>
    /// Sets type of the writer that is used to read data to
    /// serialization stream in MobileFormatter.
    /// </summary>
    public string Reader
    {
      get { return ConfigurationManager.AppSettings["CslaReader"]; }
      set { ConfigurationManager.AppSettings["CslaReader"] = value; }
    }

    /// <summary>
    /// Gets or sets a delegate reference to the method
    /// called to create instances of factory objects
    /// as requested by the MobileFactory attribute on
    /// a CSLA Light business object.
    /// </summary>
    public string MobileFactoryLoader
    {
      get { return ConfigurationManager.AppSettings["CslaMobileFactoryLoader"]; }
      set { ConfigurationManager.AppSettings["CslaMobileFactoryLoader"] = value; }
    }

    /// <summary>
    /// Sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// IObjectFactoryLoader.
    /// </summary>
    public string ObjectFactoryLoader
    {
      get { return ConfigurationManager.AppSettings["CslaObjectFactoryLoader"]; }
      set { ConfigurationManager.AppSettings["CslaObjectFactoryLoader"] = value; }
    }

    /// <summary>
    /// Sets the default transaction isolation level.
    /// </summary>
    public string DefaultTransactionIsolationLevel
    {
      get { return ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"]; }
      set { ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"] = value; }
    }

    /// <summary>
    /// Gets or sets the default transaction timeout in seconds.
    /// </summary>
    /// <value>
    /// The default transaction timeout in seconds.
    /// </value>
    public int DefaultTransactionTimeoutInSeconds
    {
      get { return int.Parse(ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"] ?? "0"); }
      set { ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"] = value.ToString(); }
    }

    /// <summary>
    /// Gets the maximum cache size
    /// </summary>
    public int PrincipalCacheSize
    {
      get
      {
        if (ConfigurationManager.AppSettings["CslaPrincipalCacheSize"] != null)
        {
          return int.Parse(ConfigurationManager.AppSettings["CslaPrincipalCacheSize"]);
        }

        return PrincipalCache.MaxCacheSize;
      }

      set { ConfigurationManager.AppSettings["CslaPrincipalCacheSize"] = value.ToString(); }
    }

    /// <summary>
    /// Get an instance of the writer that is used to write data to serialization stream
    /// Instance has to implement <see cref="ICslaWriter"/>.
    /// </summary>
    public string MobileWriter
    {
      get { return ConfigurationManager.AppSettings["CslaWriter"]; }
      set { ConfigurationManager.AppSettings["CslaWriter"] = value; }
    }

    /// <summary>
    /// DBProvider
    /// Instance has to implement <see cref="ICslaWriter"/>.
    /// </summary>
    public string DbProvider
    {
      get { return ConfigurationManager.AppSettings["dbProvider"]; }
      set { ConfigurationManager.AppSettings["dbProvider"] = value; }
    }

    /// <summary>
    /// Gets or sets the data portal configuration options
    /// </summary>
    public CslaDataPortalConfigurationOptions DataPortal { get; set; }
  }
}