//-----------------------------------------------------------------------
// <copyright file="ClaimsPrincipalFormatter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A formatter that can serialize and deserialize a ClaimsPrincipal object</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;

namespace Csla.Serialization.Mobile.CustomSerializers;

/// <summary>
/// A formatter that can serialize and deserialize a ClaimsPrincipal object.
/// </summary>
public class ClaimsPrincipalSerializer : IMobileSerializer
{
  /// <inheritdoc />
  public object Deserialize(SerializationInfo info)
  {
    var state = info.GetValue<byte[]>("s");
    using var buffer = new MemoryStream(state);
    using var reader = new BinaryReader(buffer);
    var mobile = new ClaimsPrincipal(reader);
    return mobile;
  }

  /// <inheritdoc />
  public void Serialize(object obj, SerializationInfo info)
  {
    if (obj is not ClaimsPrincipal principal)
      throw new ArgumentException("obj.GetType() != ClaimsPrincipal", nameof(obj));

    using var buffer = new MemoryStream();
    using var writer = new BinaryWriter(buffer);
    principal.WriteTo(writer);
    info.AddValue("s", buffer.ToArray());
  }
}
