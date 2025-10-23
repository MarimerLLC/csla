//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateHelper.cs" company="Marimer LLC">
// Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for serializing metastate to/from byte arrays.</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
using Csla.Core;

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
    /// <param name="mode">The state mode (Serialization or Undo).</param>
    /// <returns>Byte array containing the serialized metastate.</returns>
    public static byte[] SerializeMetastate(IMobileObject mobileObject, StateMode mode)
    {
   if (mobileObject == null)
        throw new ArgumentNullException(nameof(mobileObject));

      // Create temporary SerializationInfo
  var typeName = mobileObject.GetType().AssemblyQualifiedName ?? mobileObject.GetType().FullName ?? "Unknown";
      var info = new SerializationInfo(1, typeName);
      
      // Invoke OnGetState to capture field values
  InvokeOnGetState(mobileObject, info, mode);
      
 // Extract only the Values dictionary (not Children)
      var valueDictionary = ExtractValues(info);
 
      // Serialize to JSON
      return JsonSerializer.SerializeToUtf8Bytes(valueDictionary, JsonOptions);
    }

    /// <summary>
    /// Deserializes metastate from a byte array into an IMobileObject.
    /// </summary>
    /// <param name="mobileObject">The object to deserialize into.</param>
    /// <param name="metastate">The byte array containing serialized metastate.</param>
    /// <param name="mode">The state mode (Serialization or Undo).</param>
    public static void DeserializeMetastate(IMobileObject mobileObject, byte[] metastate, StateMode mode)
  {
if (mobileObject == null)
        throw new ArgumentNullException(nameof(mobileObject));
      if (metastate == null || metastate.Length == 0)
      throw new ArgumentException("Metastate cannot be null or empty.", nameof(metastate));

      // Deserialize from JSON
      var valueDictionary = JsonSerializer.Deserialize<Dictionary<string, MetastateValue>>(metastate, JsonOptions);
      if (valueDictionary == null)
     throw new InvalidOperationException("Failed to deserialize metastate.");
      
      // Create SerializationInfo and populate with deserialized values
      var typeName = mobileObject.GetType().AssemblyQualifiedName ?? mobileObject.GetType().FullName ?? "Unknown";
      var info = new SerializationInfo(1, typeName);
    PopulateSerializationInfo(info, valueDictionary);
      
      // Invoke OnSetState to restore field values
      InvokeOnSetState(mobileObject, info, mode);
    }

    private static void InvokeOnGetState(IMobileObject obj, SerializationInfo info, StateMode mode)
    {
      // Use reflection to call OnGetState (protected method)
      var method = obj.GetType().GetMethod("OnGetState", 
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
      
      if (method != null)
    {
        method.Invoke(obj, new object[] { info, mode });
      }
    }

    private static void InvokeOnSetState(IMobileObject obj, SerializationInfo info, StateMode mode)
    {
      // Use reflection to call OnSetState (protected method)
      var method = obj.GetType().GetMethod("OnSetState",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
    
      if (method != null)
      {
        method.Invoke(obj, new object[] { info, mode });
      }
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
        info.AddValue(kvp.Key, kvp.Value.Value, kvp.Value.IsDirty, kvp.Value.EnumTypeName);
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
