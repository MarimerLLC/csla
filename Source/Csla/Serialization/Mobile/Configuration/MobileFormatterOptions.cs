//-----------------------------------------------------------------------
// <copyright file="MobileFormatterOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for MobileFormatter configuration</summary>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
  /// Adds the custom serializers registered in the specified assemblies
  /// to <see cref="CustomSerializers"/>.
  /// </summary>
  /// <remarks>
  /// Reads each assembly's <see cref="MobileSerializerRegistrationAttribute"/>
  /// entries, which the CSLA source generator emits for types marked with
  /// <see cref="MobileSerializerAttribute"/> and serializers marked with
  /// <see cref="MobileSerializerForAttribute"/>. Each registration maps the exact
  /// type to its serializer. A registration whose serializer is already
  /// registered for the type is skipped, so calling this method more than once
  /// for an assembly has no further effect. The serializers are added after any
  /// that are already registered, which take precedence.
  /// </remarks>
  /// <param name="assemblies">The assemblies to scan.</param>
  /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> or one of its elements is <see langword="null"/>.</exception>
  public MobileFormatterOptions AddCustomSerializers(params Assembly[] assemblies)
  {
    if (assemblies is null)
      throw new ArgumentNullException(nameof(assemblies));

    foreach (var assembly in assemblies)
    {
      if (assembly is null)
        throw new ArgumentNullException(nameof(assemblies));

      foreach (var registration in assembly.GetCustomAttributes<MobileSerializerRegistrationAttribute>())
      {
        var type = registration.Type;
        var serializerType = registration.SerializerType;
        if (type is null || serializerType is null)
          continue;
        if (CustomSerializers.Any(s => s.SerializerType == serializerType && s.CanSerialize(type)))
          continue;
        CustomSerializers.Add(new RegisteredTypeMap(type, serializerType));
      }
    }
    return this;
  }

  /// <summary>
  /// Gets a value indicating whether to use strong name checks during serialization/deserialization process.
  /// </summary>
  public bool UseStrongNamesCheck { get; set; } = true;

  /// <summary>
  /// Sets type of the writer that is used to write data to 
  /// serialization stream in SerializationFormatterFactory.GetFormatter().
  /// </summary>
  public MobileFormatterOptions MobileWriter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : ICslaWriter
  {
    CslaReaderWriterFactory.WriterType = typeof(T);
    return this;
  }

  /// <summary>
  /// Sets type of the writer that is used to read data to 
  /// serialization stream in SerializationFormatterFactory.GetFormatter().
  /// </summary>
  public MobileFormatterOptions MobileReader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : ICslaReader
  {
    CslaReaderWriterFactory.ReaderType = typeof(T);
    return this;
  }

  /// <summary>
  /// Enable strong name type checking during serialization/deserilization process.
  /// </summary>
  public MobileFormatterOptions EnableStrongNamesCheck()
  {
    UseStrongNamesCheck = true;
    return this;
  }

  /// <summary>
  /// Enable strong name type checking during serialization/deserilization process.
  /// </summary>
  public MobileFormatterOptions DisableStrongNamesCheck()
  {
    UseStrongNamesCheck = false;
    return this;
  }
}
