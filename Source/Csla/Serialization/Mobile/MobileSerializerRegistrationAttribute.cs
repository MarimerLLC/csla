//-----------------------------------------------------------------------
// <copyright file="MobileSerializerRegistrationAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Registers a custom serializer for a type at the assembly level</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Serialization.Mobile;

/// <summary>
/// Registers the <see cref="IMobileSerializer"/> that <see cref="MobileFormatter"/>
/// uses for a type.
/// </summary>
/// <remarks>
/// <para>
/// The CSLA source generator emits this attribute for each type marked with
/// <see cref="MobileSerializerAttribute"/> and each type listed in a
/// <see cref="MobileSerializerForAttribute"/>. You can also write it yourself, for
/// example to map a third-party type to <c>PocoSerializer&lt;T&gt;</c>:
/// </para>
/// <code>
/// [assembly: MobileSerializerRegistration(typeof(Gadget), typeof(PocoSerializer&lt;Gadget&gt;))]
/// </code>
/// <para>
/// <see cref="Configuration.MobileFormatterOptions.AddCustomSerializers(System.Reflection.Assembly[])"/>
/// reads these attributes to register the serializers at run time, and the CSLA
/// analyzers read them so they don't report the types as not serializable. The
/// mapping is for the exact type: derived types are not covered.
/// </para>
/// </remarks>
/// <param name="type">The type to serialize.</param>
/// <param name="serializerType">The serializer type, which implements <see cref="IMobileSerializer"/>.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class MobileSerializerRegistrationAttribute(
  Type type,
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type serializerType) : Attribute
{
  /// <summary>
  /// Gets the type to serialize.
  /// </summary>
  public Type Type { get; } = type;

  /// <summary>
  /// Gets the serializer type.
  /// </summary>
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
  public Type SerializerType { get; } = serializerType;
}
