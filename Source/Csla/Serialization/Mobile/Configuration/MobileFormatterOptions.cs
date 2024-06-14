﻿//-----------------------------------------------------------------------
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
}

public interface ITypeMap
{
  Type OriginalType { get; }
  Type SerializerType { get; }
  Func<Type, bool> CanSerialize { get; set; }
}
/// <summary>
/// Maps a type to a serializer type.
/// </summary>
public class TypeMap<T, S> : ITypeMap
  where S : IMobileSerializer
{
  /// <summary>
  /// Creates an instance of the class.
  /// </summary>
  public TypeMap()
  {
    CanSerialize = (t) => t == OriginalType;
  }

  /// <summary>
  /// Gets or sets the original type.
  /// </summary>
  public Type OriginalType => typeof(T);
  /// <summary>
  /// Gets or sets the serializer type.
  /// </summary>
  public Type SerializerType => typeof(S);
  /// <summary>
  /// Gets or sets a function that determines 
  /// if the type can be serialized.
  /// </summary>
  public Func<Type, bool> CanSerialize { get; set; }
}
