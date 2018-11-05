using Csla.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public class CslaConfiguration : ICslaConfiguration
  {
    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    public CslaConfiguration()
    {
      DataPortal = new CslaDataPortalConfiguration(this);
      Data = new CslaDataConfiguration(this);
      Security = new CslaSecurityConfiguration(this);
      Serialization = new CslaSerializationConfiguration(this);
    }

#if !NETSTANDARD2_0
    /// <summary>
    /// Sets the web context manager.
    /// </summary>
    /// <param name="contextManager">Web context manager instance</param>
    /// <remarks>
    /// Will use default WebContextManager. 
    /// Only need to set for non-default WebContextManager.
    /// </remarks>
    /// <returns></returns>
    public CslaConfiguration WebContextManager(IContextManager contextManager)
    {
      ApplicationContext.WebContextManager = contextManager;
      return this;
    }
#endif

    /// <summary>
    /// Gets a reference to the data portal configuration object.
    /// </summary>
    public CslaDataPortalConfiguration DataPortal { get; private set; }

    /// <summary>
    /// Gets a reference to the data configuration object.
    /// </summary>
    public CslaDataConfiguration Data { get; private set; }

    /// <summary>
    /// Gets a reference to the data configuration object.
    /// </summary>
    public CslaSecurityConfiguration Security { get; private set; }

    /// <summary>
    /// Gets a reference to the data configuration object.
    /// </summary>
    public CslaSerializationConfiguration Serialization { get; private set; }

    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    public CslaConfiguration UseReflectionFallback(bool value)
    {
      ApplicationContext.UseReflectionFallback = value;
      return this;
    }

    /// <summary>
    /// Sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    /// <param name="mode">Property changed mode</param>
    public CslaConfiguration PropertyChangedMode(ApplicationContext.PropertyChangedModes mode)
    {
      ApplicationContext.PropertyChangedMode = mode;
      return this;
    }

    /// <summary>
    /// Sets the RuleSet name to use for static HasPermission calls.
    /// </summary>
    /// <param name="ruleSet">The rule set.</param>
    public CslaConfiguration RuleSet(string ruleSet)
    {
      ApplicationContext.RuleSet = ruleSet;
      return this;
    }

    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    /// <param name="typeName">Factory type name</param>
    public CslaConfiguration PropertyInfoFactory(string typeName)
    {
      ConfigurationManager.AppSettings["CslaPropertyInfoFactory"] = typeName;
      return this;
    }
  }
}