//-----------------------------------------------------------------------
// <copyright file="SerializationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for serialization.</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for serialization.
  /// </summary>
  /// <param name="cslaOptions">The CSLA options.</param>
  /// <exception cref="ArgumentNullException"><paramref name="cslaOptions"/> is <see langword="null"/>.</exception>
  public class SerializationOptions(CslaOptions cslaOptions)
  {
    /// <summary>
    /// Gets the CSLA .NET configuration options.
    /// </summary>
    protected CslaOptions CslaOptions { get; } = Guard.NotNull(cslaOptions);

    /// <summary>
    /// Gets the current services collection.
    /// </summary>
    public IServiceCollection Services => CslaOptions.Services;

    /// <summary>
    /// Sets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc). Default is MobileFormatter.
    /// </summary>
    public SerializationOptions UseSerializationFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : ISerializationFormatter
    {
      CslaOptions.Services.AddTransient(typeof(ISerializationFormatter), typeof(T));
      return this;
    }
  }
}
