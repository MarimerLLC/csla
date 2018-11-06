//-----------------------------------------------------------------------
// <copyright file="CslaSerializationConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Use this type to configure the settings</summary>
//-----------------------------------------------------------------------
namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaSerializationConfiguration
  /// </summary>
  public static class CslaSerializationConfigurationExtension
  {
    /// <summary>
    /// Extension method for CslaSerializationConfiguration
    /// </summary>
    public static CslaSerializationConfiguration Serialization(this ICslaConfiguration config)
    {
      return new CslaSerializationConfiguration(config);
    }
  }

  /// <summary>
  /// Use this type to configure the settings for serialization.
  /// </summary>
  public class CslaSerializationConfiguration
  {
    private ICslaConfiguration RootConfiguration { get; set; }

    internal CslaSerializationConfiguration(ICslaConfiguration root)
    {
      RootConfiguration = root;
    }

    /// <summary>
    /// Sets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    /// <param name="formatterName">Formatter name (one of MobileFormatter, 
    /// BinaryFormatter, NetDataContractSerializer, or type name)</param>
    public ICslaConfiguration SerializationFormatter(string formatterName)
    {
      ConfigurationManager.AppSettings["CslaSerializationFormatter"] = formatterName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets type of the writer that is used to write data to 
    /// serialization stream in MobileFormatter.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration MobileWriter(string typeName)
    {
      ConfigurationManager.AppSettings["CslaWriter"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets type of the writer that is used to read data to 
    /// serialization stream in MobileFormatter.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration MobileReader(string typeName)
    {
      ConfigurationManager.AppSettings["CslaReader"] = typeName;
      return RootConfiguration;
    }
  }
}
