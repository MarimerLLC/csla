//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateHelper.cs" company="Marimer LLC">
// Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for serializing metastate to/from byte arrays.</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Helper class for serializing and deserializing IMobileObject metastate.
  /// </summary>
  internal static class MobileObjectMetastateHelper
  {
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

      using var stream = new MemoryStream();
      using var writer = new BinaryWriter(stream);

      // Write the number of values
      writer.Write(info.Values.Count);

      // Write each value
      foreach (var kvp in info.Values)
      {
        writer.Write(kvp.Key);
        WriteFieldData(writer, kvp.Value);
      }

      return stream.ToArray();
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

      var typeName = mobileObject.GetType().AssemblyQualifiedName ?? mobileObject.GetType().FullName ?? "Unknown";
      var info = new SerializationInfo(1, typeName);

      using var stream = new MemoryStream(metastate);
      using var reader = new BinaryReader(stream);

      // Read the number of values
      int count = reader.ReadInt32();

      // Read each value
      for (int i = 0; i < count; i++)
      {
        string key = reader.ReadString();
        var fieldData = ReadFieldData(reader);
        info.AddValue(key, fieldData.Value, fieldData.IsDirty, fieldData.EnumTypeName);
      }

      mobileObject.SetState(info);
    }

    private static void WriteFieldData(BinaryWriter writer, SerializationInfo.FieldData fieldData)
    {
      // Write IsDirty flag
      writer.Write(fieldData.IsDirty);

      // Write EnumTypeName (null or string)
      if (fieldData.EnumTypeName == null)
      {
        writer.Write(false);
      }
      else
      {
        writer.Write(true);
        writer.Write(fieldData.EnumTypeName);
      }

      // Write the value with type information using shared helper
      BinaryValueHelper.WriteValue(fieldData.Value, writer);
    }

    private static FieldDataResult ReadFieldData(BinaryReader reader)
    {
      // Read IsDirty flag
      bool isDirty = reader.ReadBoolean();

      // Read EnumTypeName
      string? enumTypeName = null;
      bool hasEnumTypeName = reader.ReadBoolean();
      if (hasEnumTypeName)
      {
        enumTypeName = reader.ReadString();
      }

      // Read the value using shared helper
      var value = BinaryValueHelper.ReadValue(reader);

      return new FieldDataResult
      {
        Value = value,
        IsDirty = isDirty,
        EnumTypeName = enumTypeName
      };
    }

    /// <summary>
    /// Represents the result of reading a field value.
    /// </summary>
    private struct FieldDataResult
    {
      public object? Value { get; set; }
      public bool IsDirty { get; set; }
      public string? EnumTypeName { get; set; }
    }
  }
}
