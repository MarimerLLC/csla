//-----------------------------------------------------------------------
// <copyright file="SerializationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for serialization.</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;

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
    public SerializationOptions UseSerializationFormatter<T>() where T : ISerializationFormatter
    {
      SerializationFormatterType = typeof(T);
      return this;
    }
    
    /// <summary>
    /// Gets the serialization formatter type.
    /// </summary>
    public Type SerializationFormatterType { get; private set; }

    /// <summary>
    /// Options for the serialization formatter.
    /// </summary>
    public object FormatterOptions { get; set; }
  }
}
