//-----------------------------------------------------------------------
// <copyright file="SerializationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for serialization.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;

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
    public SerializationOptions MobileWriter<T>() where T : ICslaWriter
    {
      CslaReaderWriterFactory.WriterType = typeof(T);
      return this;
    }

    /// <summary>
    /// Sets type of the writer that is used to read data to 
    /// serialization stream in SerializationFormatterFactory.GetFormatter().
    /// </summary>
    public SerializationOptions MobileReader<T>() where T : ICslaReader
    {
      CslaReaderWriterFactory.ReaderType = typeof(T);
      return this;
    }
  }
}
