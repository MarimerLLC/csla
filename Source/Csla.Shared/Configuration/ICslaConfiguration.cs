using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public interface ICslaConfiguration
  {
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
    ICslaConfiguration WebContextManager(IContextManager contextManager);
#endif
    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    ICslaConfiguration UseReflectionFallback(bool value);
    /// <summary>
    /// Sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    /// <param name="mode">Property changed mode</param>
    ICslaConfiguration PropertyChangedMode(ApplicationContext.PropertyChangedModes mode);
    /// <summary>
    /// Sets the RuleSet name to use for static HasPermission calls.
    /// </summary>
    /// <param name="ruleSet">The rule set.</param>
    ICslaConfiguration RuleSet(string ruleSet);
    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    /// <param name="typeName">Factory type name</param>
    ICslaConfiguration PropertyInfoFactory(string typeName);
    /// <summary>
    /// Resets any ApplicationContext settings so they 
    /// re-read their configuration from AppSettings
    /// on next use.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    ICslaConfiguration SettingsChanged();
  }
}
