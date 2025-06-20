//-----------------------------------------------------------------------
// <copyright file="ClaimsPrincipalFormatter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A formatter that can serialize and deserialize a ClaimsPrincipal object</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Properties;

namespace Csla.Serialization.Mobile.CustomSerializers;


#if NET8_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
/// <summary>
/// A formatter that can serialize and deserialize a ClaimsPrincipal object.
/// </summary>
internal class ClaimsPrincipalSerializer : IMobileSerializer
{
  /// <inheritdoc />
  public static bool CanSerialize(Type type) => type == typeof(ClaimsPrincipal);

  /// <inheritdoc />
  public object Deserialize(SerializationInfo info)
  {
    if (info is null)
      throw new ArgumentNullException(nameof(info));

    var state = info.GetValue<byte[]>("s") ?? throw new System.Runtime.Serialization.SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, typeof(ClaimsPrincipal)));
    using var buffer = new MemoryStream(state);
    using var reader = new BinaryReader(buffer);
    var mobile = new ClaimsPrincipal(reader);
    return mobile;
  }

  /// <inheritdoc />
  public void Serialize(object obj, SerializationInfo info)
  {
    if (obj is null)
      throw new ArgumentNullException(nameof(obj));
    if (info is null)
      throw new ArgumentNullException(nameof(info));
    if (obj is not ClaimsPrincipal principal)
      throw new ArgumentException($"{obj.GetType()} != {nameof(ClaimsPrincipal)}", nameof(obj));

    using var buffer = new MemoryStream();
    using var writer = new BinaryWriter(buffer);
    principal.WriteTo(writer);
    info.AddValue("s", buffer.ToArray());
  }
}
#else
using System.Text.Json;

/// <summary>
/// A formatter that can serialize and deserialize a ClaimsPrincipal object.
/// </summary>
internal class ClaimsPrincipalSerializer : IMobileSerializer
{
  /// <inheritdoc />
  public static bool CanSerialize(Type type) => type == typeof(ClaimsPrincipal);

  /// <inheritdoc />
  public object Deserialize(SerializationInfo info)
  {
    if (info is null)
      throw new ArgumentNullException(nameof(info));

    var json = info.GetValue<string>("s") ?? throw new System.Runtime.Serialization.SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, typeof(ClaimsPrincipal)));
    var dto = JsonSerializer.Deserialize<PrincipalDto>(json);
    if (dto is null)
      throw new System.Runtime.Serialization.SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, typeof(ClaimsPrincipal)));

    var identities = new List<ClaimsIdentity>();
    foreach (var i in dto.Identities)
    {
      var claims = i.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)).ToList();
      var identity = new ClaimsIdentity(claims, i.AuthenticationType)
      {
        Label = i.Label
      };
      identities.Add(identity);
    }
    var result = new ClaimsPrincipal(identities);
    return result;
  }

  /// <inheritdoc />
  public void Serialize(object obj, SerializationInfo info)
  {
    if (obj is null)
      throw new ArgumentNullException(nameof(obj));
    if (info is null)
      throw new ArgumentNullException(nameof(info));
    if (obj is not ClaimsPrincipal principal)
      throw new ArgumentException($"{obj.GetType()} != {nameof(ClaimsPrincipal)}", nameof(obj));

    var dto = new PrincipalDto();
    foreach (var i in principal.Identities)
    {
      dto.Identities.Add(new IdentityDto
      {
        AuthenticationType = i.AuthenticationType,
        Label = i.Label,
        Claims = i.Claims.Select(c => new ClaimDto
        {
          Type = c.Type,
          Value = c.Value,
          ValueType = c.ValueType
        }).ToList()
      });
    }
    info.AddValue("s", JsonSerializer.Serialize(dto));
  }

  private class PrincipalDto
  {
    public List<IdentityDto> Identities { get; set; } = [];
  }

  private class IdentityDto
  {
    public string? AuthenticationType { get; set; }
    public string? Label { get; set; }
    public List<ClaimDto>? Claims { get; set; }
  }

  private class ClaimDto
  {
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? ValueType { get; set; }
  }
}
#endif
