//-----------------------------------------------------------------------
// <copyright file="SerializationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for serialization.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for serialization.
  /// </summary>
  public class SerializationOptions
  {
    /// <summary>
    /// Sets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc). Default is MobileFormatter.
    /// </summary>
    /// <param name="formatterType">Serialization formatter type.</param>
    public SerializationOptions SerializationFormatter(Type formatterType)
    {
      ApplicationContext.SerializationFormatter = formatterType;
      return this;
    }

    /// <summary>
    /// Sets type of the writer that is used to write data to 
    /// serialization stream in SerializationFormatterFactory.GetFormatter().
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public SerializationOptions MobileWriter(string typeName)
    {
      ConfigurationManager.AppSettings["CslaWriter"] = typeName;
      return this;
    }

    /// <summary>
    /// Sets type of the writer that is used to read data to 
    /// serialization stream in SerializationFormatterFactory.GetFormatter().
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public SerializationOptions MobileReader(string typeName)
    {
      ConfigurationManager.AppSettings["CslaReader"] = typeName;
      return this;
    }
  }
}
