//-----------------------------------------------------------------------
// <copyright file="MobileSerializerForAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Declares the types a custom serializer handles</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile;

/// <summary>
/// Declares the types that the attributed <see cref="IMobileSerializer"/>
/// serializes. Use it for types you don't own and can't attribute with
/// <see cref="MobileSerializerAttribute"/>.
/// </summary>
/// <remarks>
/// <para>
/// The CSLA source generator turns this attribute into assembly-level
/// <see cref="MobileSerializerRegistrationAttribute"/> entries, which the CSLA
/// analyzers read so they don't report the types as not serializable. To use the
/// serializer at run time, register the assembly with
/// <see cref="Configuration.MobileFormatterOptions.AddCustomSerializers(System.Reflection.Assembly[])"/>.
/// </para>
/// <para>
/// Each mapping is for the exact type: derived types are not covered.
/// </para>
/// </remarks>
/// <param name="types">The types the serializer handles.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MobileSerializerForAttribute(params Type[] types) : Attribute
{
  /// <summary>
  /// Gets the types the serializer handles.
  /// </summary>
  public Type[] Types { get; } = types ?? [];
}
