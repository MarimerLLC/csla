using System;
using System.Configuration;

namespace CSLA
{
  /// <summary>
  /// Provides access to configuration settings.
  /// </summary>
  public class ConfigurationSettings
  {
    static ConfigurationSettingsList _settings = 
      new ConfigurationSettingsList();

    /// <summary>
    /// Gets configuration settings in the configuration section.
    /// </summary>
    public static ConfigurationSettingsList AppSettings
    {
      get
      {
        return _settings;
      }
    }
  }

  public class ConfigurationSettingsList
  {
    public string this [string name]
    {
      get
      {
        string val = 
          System.Configuration.ConfigurationSettings.AppSettings[name];
        if(val == null)
          return string.Empty;
        else
          return val;
      }
    }
  }
}
