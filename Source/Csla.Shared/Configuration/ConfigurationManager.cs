using System.Collections.Specialized;

/// <summary>
/// Configuration types for CSLA .NET.
/// </summary>
namespace Csla.Configuration
{
  /// <summary>
  /// ConfigurationManager that abstracts underlying configuration
  /// management implementations and infrastructure.
  /// </summary>
  public static class ConfigurationManager
  {
#if NETSTANDARD2_0
    private static NameValueCollection _settings = new NameValueCollection();
#else
    private static NameValueCollection _settings = System.Configuration.ConfigurationManager.AppSettings;
#endif

    /// <summary>
    /// Gets or sets the app settings for the application's default settings.
    /// </summary>
    public static NameValueCollection AppSettings
    {
      get
      {
        return _settings;
      }
      set
      {
        _settings = value;
        ApplicationContext.SettingsChanged();
      }
    }
  }
}
