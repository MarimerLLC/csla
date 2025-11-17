//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateHelper.cs" company="Marimer LLC">
// Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for serializing metastate to/from byte arrays.</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Helper class for serializing and deserializing IMobileObject metastate.
  /// </summary>
  internal static class MobileObjectMetastateHelper
  {
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
#if NET5_0_OR_GREATER
      DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
#endif
      WriteIndented = false,
      IncludeFields = false
    };

    /// <summary>
    /// Serializes the metastate of an IMobileObject to a byte array.
    /// </summary>
    /// <param name="mobileObject">The object to serialize.</param>
    /// <returns>Byte array containing the serialized metastate.</returns>
    public static byte[] SerializeMetastate(IMobileObject mobileObject)
    {
      if (mobileObject == null)
        throw new ArgumentNullException(nameof(mobileObject));

      var typeName = mobileObject.GetType().AssemblyQualifiedName ?? mobileObject.GetType().FullName ?? "Unknown";
      var info = new SerializationInfo(1, typeName);

      mobileObject.GetState(info);

      var valueDictionary = ExtractValues(info);

      return JsonSerializer.SerializeToUtf8Bytes(valueDictionary, JsonOptions);
    }

    /// <summary>
    /// Deserializes metastate from a byte array into an IMobileObject.
    /// </summary>
    /// <param name="mobileObject">The object to deserialize into.</param>
    /// <param name="metastate">The byte array containing serialized metastate.</param>
    public static void DeserializeMetastate(IMobileObject mobileObject, byte[] metastate)
    {
      if (mobileObject == null)
        throw new ArgumentNullException(nameof(mobileObject));
      if (metastate == null)
        throw new ArgumentNullException(nameof(metastate));
      if (metastate.Length == 0)
        throw new ArgumentException("Metastate cannot be empty.", nameof(metastate));

      var valueDictionary = JsonSerializer.Deserialize<Dictionary<string, MetastateValue>>(metastate, JsonOptions)
        ?? throw new InvalidOperationException("Failed to deserialize metastate.");

      var typeName = mobileObject.GetType().AssemblyQualifiedName ?? mobileObject.GetType().FullName ?? "Unknown";
      var info = new SerializationInfo(1, typeName);
      PopulateSerializationInfo(info, valueDictionary);

      mobileObject.SetState(info);
    }

    private static Dictionary<string, MetastateValue> ExtractValues(SerializationInfo info)
    {
      var result = new Dictionary<string, MetastateValue>();
      
      // Only extract from Values dictionary, not Children
      foreach (var kvp in info.Values)
      {
        result[kvp.Key] = new MetastateValue
        {
          Value = kvp.Value.Value,
          EnumTypeName = kvp.Value.EnumTypeName,
          IsDirty = kvp.Value.IsDirty
        };
      }

      return result;
    }

    private static void PopulateSerializationInfo(SerializationInfo info, 
      Dictionary<string, MetastateValue> values)
    {
      foreach (var kvp in values)
      {
        var value = kvp.Value.Value;
        
        // Convert JsonElement to appropriate type if needed
        if (value is JsonElement jsonElement)
        {
          value = ConvertJsonElement(jsonElement);
        }
        
        info.AddValue(kvp.Key, value, kvp.Value.IsDirty, kvp.Value.EnumTypeName);
      }
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
      switch (element.ValueKind)
      {
        case JsonValueKind.String:
          return element.GetString();
        case JsonValueKind.Number:
          if (element.TryGetInt32(out int intValue))
            return intValue;
          if (element.TryGetInt64(out long longValue))
            return longValue;
          if (element.TryGetDouble(out double doubleValue))
            return doubleValue;
          return element.GetDecimal();
        case JsonValueKind.True:
          return true;
        case JsonValueKind.False:
          return false;
        case JsonValueKind.Null:
        case JsonValueKind.Undefined:
          return null;
        default:
          // For complex types, return the element as-is and let the normal
          // conversion logic handle it
          return element;
      }
    }

    /// <summary>
    /// Represents a serialized value with metadata.
    /// </summary>
    private class MetastateValue
    {
      public object? Value { get; set; }
      public string? EnumTypeName { get; set; }
      public bool IsDirty { get; set; }
    }
  }
}
