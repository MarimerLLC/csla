//-----------------------------------------------------------------------
// <copyright file="SerializationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for serialization.</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for serialization.
  /// </summary>
  /// <param name="cslaOptions">The CSLA options.</param>
  public class SerializationOptions(CslaOptions cslaOptions)
  {
    /// <summary>
    /// Gets the CSLA .NET configuration options.
    /// </summary>
    protected CslaOptions CslaOptions => cslaOptions;

    /// <summary>
    /// Sets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc). Default is MobileFormatter.
    /// </summary>
    public SerializationOptions UseSerializationFormatter<T>() where T : ISerializationFormatter
    {
      CslaOptions.Services.TryAddTransient(typeof(ISerializationFormatter), typeof(T));
      return this;
    }

    /// <summary>
    /// Gets the serialization formatter type used by CSLA .NET.
    /// </summary>
    public Type SerializationFormatterType 
      => CslaOptions.Services.FirstOrDefault(_ => _.ServiceType == typeof(ISerializationFormatter))?.ServiceType;

    /// <summary>
    /// Options for the serialization formatter.
    /// </summary>
    public object FormatterOptions { get; set; }
  }
}
