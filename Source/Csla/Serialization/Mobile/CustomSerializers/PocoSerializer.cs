//-----------------------------------------------------------------------
// <copyright file="ClaimsPrincipalFormatter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A formatter that can serialize and deserialize a POCO</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile.CustomSerializers;

/// <summary>
/// A formatter that can serialize and deserialize a POCO
/// by using System.Text.Json.
/// </summary>
public class PocoSerializer<T> : IMobileSerializer
{
  /// <inheritdoc />
  public static bool CanSerialize(Type type) => type == typeof(T);

  /// <inheritdoc />
  public object Deserialize(SerializationInfo info)
  {
    var json = info.GetValue<string>("s");
    return JsonSerializer.Deserialize<T>(json);
  }

  /// <inheritdoc />
  public void Serialize(object obj, SerializationInfo info)
  {
    if (obj is null)
      throw new ArgumentNullException(nameof(obj));
    if (!CanSerialize(obj.GetType()))
      throw new ArgumentException($"{obj.GetType()} != [SerializablePoco]", nameof(obj));

    var json = JsonSerializer.Serialize(obj);
    info.AddValue("s", json);
  }
}
