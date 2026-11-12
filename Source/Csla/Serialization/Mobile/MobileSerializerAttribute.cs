//-----------------------------------------------------------------------
// <copyright file="MobileSerializerAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Declares the custom serializer MobileFormatter uses for a type</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Serialization.Mobile;

/// <summary>
/// Declares the <see cref="IMobileSerializer"/> that <see cref="MobileFormatter"/>
/// uses to serialize the attributed type.
/// </summary>
/// <remarks>
/// <para>
/// The CSLA source generator turns this attribute into an assembly-level
/// <see cref="MobileSerializerRegistrationAttribute"/>, which the CSLA analyzers
/// read so they don't report the type as not serializable. To use the serializer
/// at run time, register the assembly with
/// <see cref="Configuration.MobileFormatterOptions.AddCustomSerializers(System.Reflection.Assembly[])"/>.
/// </para>
/// <para>
/// The mapping is for the exact type: types derived from the attributed type
/// are not covered.
/// </para>
/// </remarks>
/// <param name="serializerType">
/// A non-abstract, non-generic or closed generic class that implements
/// <see cref="IMobileSerializer"/> and has a public constructor.
/// </param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class MobileSerializerAttribute(
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type serializerType) : Attribute
{
  /// <summary>
  /// Gets the serializer type.
  /// </summary>
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
  public Type SerializerType { get; } = serializerType;
}
