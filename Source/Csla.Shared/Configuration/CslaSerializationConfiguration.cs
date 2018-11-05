using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for serialization.
  /// </summary>
  public class CslaSerializationConfiguration
  {
    private CslaConfiguration RootConfiguration { get; set; }

    internal CslaSerializationConfiguration(CslaConfiguration root)
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
    public CslaConfiguration SerializationFormatter(string formatterName)
    {
      ConfigurationManager.AppSettings["CslaSerializationFormatter"] = formatterName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets type of the writer that is used to write data to 
    /// serialization stream in MobileFormatter.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public CslaConfiguration MobileWriter(string typeName)
    {
      ConfigurationManager.AppSettings["CslaWriter"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets type of the writer that is used to read data to 
    /// serialization stream in MobileFormatter.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public CslaConfiguration MobileReader(string typeName)
    {
      ConfigurationManager.AppSettings["CslaReader"] = typeName;
      return RootConfiguration;
    }
  }
}
