//-----------------------------------------------------------------------
// <copyright file="MobileFormatterOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for MobileFormatter configuration</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;

namespace Csla.Configuration;

/// <summary>
/// Options for MobileFormatter configuration
/// </summary>
public class MobileFormatterOptions
{
  /// <summary>
  /// Gets the list of custom serializers.
  /// </summary>
  public List<ITypeMap> CustomSerializers { get; } = [];
  
  /// <summary>
  /// Gets a value indicating whether to use strong name checks during serialization/deserialization process.
  /// </summary>
  public bool UseStrongNamesCheck { get; set; } = true;

  /// <summary>
  /// Sets type of the writer that is used to write data to 
  /// serialization stream in SerializationFormatterFactory.GetFormatter().
  /// </summary>
  public MobileFormatterOptions MobileWriter<T>() where T : ICslaWriter
  {
    CslaReaderWriterFactory.WriterType = typeof(T);
    return this;
  }

  /// <summary>
  /// Sets type of the writer that is used to read data to 
  /// serialization stream in SerializationFormatterFactory.GetFormatter().
  /// </summary>
  public MobileFormatterOptions MobileReader<T>() where T : ICslaReader
  {
    CslaReaderWriterFactory.ReaderType = typeof(T);
    return this;
  }

  /// <summary>
  /// Set strong name type checking settings during serialization/deserilization process.
  /// </summary>
  /// <param name="check">If true check for strong names during serialization/deserilization process.</param>
  public MobileFormatterOptions StrongNamesCheck(bool check)
  {
    UseStrongNamesCheck = check;
    return this;
  }
}
