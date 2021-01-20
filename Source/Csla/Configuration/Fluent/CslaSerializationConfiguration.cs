//-----------------------------------------------------------------------
// <copyright file="CslaSerializationConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
      return new CslaSerializationConfiguration();
    }
  }

  /// <summary>
  /// Use this type to configure the settings for serialization.
  /// </summary>
  public class CslaSerializationConfiguration
  {
    /// <summary>
    /// Sets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    /// <param name="formatterName">Formatter name (one of MobileFormatter, 
    /// BinaryFormatter, NetDataContractSerializer, or type name)</param>
    public CslaSerializationConfiguration SerializationFormatter(string formatterName)
    {
      ConfigurationManager.AppSettings["CslaSerializationFormatter"] = formatterName;
      return this;
    }

    /// <summary>
    /// Sets type of the writer that is used to write data to 
    /// serialization stream in SerializationFormatterFactory.GetFormatter().
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public CslaSerializationConfiguration MobileWriter(string typeName)
    {
      ConfigurationManager.AppSettings["CslaWriter"] = typeName;
      return this;
    }

    /// <summary>
    /// Sets type of the writer that is used to read data to 
    /// serialization stream in SerializationFormatterFactory.GetFormatter().
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public CslaSerializationConfiguration MobileReader(string typeName)
    {
      ConfigurationManager.AppSettings["CslaReader"] = typeName;
      return this;
    }
  }
}
