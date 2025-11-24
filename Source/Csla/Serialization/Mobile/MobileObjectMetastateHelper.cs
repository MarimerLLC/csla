//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateHelper.cs" company="Marimer LLC">
// Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for serializing metastate to/from byte arrays.</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

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
        WriteValue(writer, kvp.Value);
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
        var fieldData = ReadValue(reader);
        info.AddValue(key, fieldData.Value, fieldData.IsDirty, fieldData.EnumTypeName);
      }

      mobileObject.SetState(info);
    }

    private static void WriteValue(BinaryWriter writer, SerializationInfo.FieldData fieldData)
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

      // Write the value with type information
      if (fieldData.Value == null)
      {
        writer.Write((byte)ValueType.Null);
      }
      else
      {
        var value = fieldData.Value;
        var valueType = value.GetType();

        if (valueType == typeof(bool))
        {
          writer.Write((byte)ValueType.Boolean);
          writer.Write((bool)value);
        }
        else if (valueType == typeof(byte))
        {
          writer.Write((byte)ValueType.Byte);
          writer.Write((byte)value);
        }
        else if (valueType == typeof(sbyte))
        {
          writer.Write((byte)ValueType.SByte);
          writer.Write((sbyte)value);
        }
        else if (valueType == typeof(char))
        {
          writer.Write((byte)ValueType.Char);
          writer.Write((char)value);
        }
        else if (valueType == typeof(short))
        {
          writer.Write((byte)ValueType.Int16);
          writer.Write((short)value);
        }
        else if (valueType == typeof(ushort))
        {
          writer.Write((byte)ValueType.UInt16);
          writer.Write((ushort)value);
        }
        else if (valueType == typeof(int))
        {
          writer.Write((byte)ValueType.Int32);
          writer.Write((int)value);
        }
        else if (valueType == typeof(uint))
        {
          writer.Write((byte)ValueType.UInt32);
          writer.Write((uint)value);
        }
        else if (valueType == typeof(long))
        {
          writer.Write((byte)ValueType.Int64);
          writer.Write((long)value);
        }
        else if (valueType == typeof(ulong))
        {
          writer.Write((byte)ValueType.UInt64);
          writer.Write((ulong)value);
        }
        else if (valueType == typeof(float))
        {
          writer.Write((byte)ValueType.Single);
          writer.Write((float)value);
        }
        else if (valueType == typeof(double))
        {
          writer.Write((byte)ValueType.Double);
          writer.Write((double)value);
        }
        else if (valueType == typeof(decimal))
        {
          writer.Write((byte)ValueType.Decimal);
          writer.Write((decimal)value);
        }
        else if (valueType == typeof(string))
        {
          writer.Write((byte)ValueType.String);
          writer.Write((string)value);
        }
        else if (valueType == typeof(DateTime))
        {
          writer.Write((byte)ValueType.DateTime);
          writer.Write(((DateTime)value).ToBinary());
        }
        else if (valueType == typeof(DateTimeOffset))
        {
          writer.Write((byte)ValueType.DateTimeOffset);
          var dto = (DateTimeOffset)value;
          writer.Write(dto.DateTime.ToBinary());
          writer.Write(dto.Offset.Ticks);
        }
        else if (valueType == typeof(TimeSpan))
        {
          writer.Write((byte)ValueType.TimeSpan);
          writer.Write(((TimeSpan)value).Ticks);
        }
        else if (valueType == typeof(Guid))
        {
          writer.Write((byte)ValueType.Guid);
          writer.Write(((Guid)value).ToByteArray());
        }
        else if (valueType == typeof(byte[]))
        {
          writer.Write((byte)ValueType.ByteArray);
          var bytes = (byte[])value;
          writer.Write(bytes.Length);
          writer.Write(bytes);
        }
        else if (valueType == typeof(char[]))
        {
          writer.Write((byte)ValueType.CharArray);
          var chars = (char[])value;
          writer.Write(chars.Length);
          writer.Write(chars);
        }
#if NET8_0_OR_GREATER
        else if (valueType == typeof(DateOnly))
        {
          writer.Write((byte)ValueType.DateOnly);
          writer.Write(((DateOnly)value).DayNumber);
        }
        else if (valueType == typeof(TimeOnly))
        {
          writer.Write((byte)ValueType.TimeOnly);
          writer.Write(((TimeOnly)value).Ticks);
        }
#endif
        else
        {
          throw new NotSupportedException($"Type {valueType.FullName} is not supported for metastate serialization.");
        }
      }
    }

    private static FieldDataResult ReadValue(BinaryReader reader)
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

      // Read the value type
      var valueType = (ValueType)reader.ReadByte();
      object? value = null;

      switch (valueType)
      {
        case ValueType.Null:
          value = null;
          break;
        case ValueType.Boolean:
          value = reader.ReadBoolean();
          break;
        case ValueType.Byte:
          value = reader.ReadByte();
          break;
        case ValueType.SByte:
          value = reader.ReadSByte();
          break;
        case ValueType.Char:
          value = reader.ReadChar();
          break;
        case ValueType.Int16:
          value = reader.ReadInt16();
          break;
        case ValueType.UInt16:
          value = reader.ReadUInt16();
          break;
        case ValueType.Int32:
          value = reader.ReadInt32();
          break;
        case ValueType.UInt32:
          value = reader.ReadUInt32();
          break;
        case ValueType.Int64:
          value = reader.ReadInt64();
          break;
        case ValueType.UInt64:
          value = reader.ReadUInt64();
          break;
        case ValueType.Single:
          value = reader.ReadSingle();
          break;
        case ValueType.Double:
          value = reader.ReadDouble();
          break;
        case ValueType.Decimal:
          value = reader.ReadDecimal();
          break;
        case ValueType.String:
          value = reader.ReadString();
          break;
        case ValueType.DateTime:
          value = DateTime.FromBinary(reader.ReadInt64());
          break;
        case ValueType.DateTimeOffset:
          var dt = DateTime.FromBinary(reader.ReadInt64());
          var offset = new TimeSpan(reader.ReadInt64());
          value = new DateTimeOffset(dt, offset);
          break;
        case ValueType.TimeSpan:
          value = new TimeSpan(reader.ReadInt64());
          break;
        case ValueType.Guid:
          value = new Guid(reader.ReadBytes(16));
          break;
        case ValueType.ByteArray:
          int byteArrayLength = reader.ReadInt32();
          value = reader.ReadBytes(byteArrayLength);
          break;
        case ValueType.CharArray:
          int charArrayLength = reader.ReadInt32();
          value = reader.ReadChars(charArrayLength);
          break;
#if NET8_0_OR_GREATER
        case ValueType.DateOnly:
          value = DateOnly.FromDayNumber(reader.ReadInt32());
          break;
        case ValueType.TimeOnly:
          value = new TimeOnly(reader.ReadInt64());
          break;
#endif
        default:
          throw new NotSupportedException($"ValueType {valueType} is not supported for metastate deserialization.");
      }

      return new FieldDataResult
      {
        Value = value,
        IsDirty = isDirty,
        EnumTypeName = enumTypeName
      };
    }

    /// <summary>
    /// Enum representing the type of value being serialized.
    /// </summary>
    private enum ValueType : byte
    {
      Null = 0,
      Boolean = 1,
      Byte = 2,
      SByte = 3,
      Char = 4,
      Int16 = 5,
      UInt16 = 6,
      Int32 = 7,
      UInt32 = 8,
      Int64 = 9,
      UInt64 = 10,
      Single = 11,
      Double = 12,
      Decimal = 13,
      String = 14,
      DateTime = 15,
      DateTimeOffset = 16,
      TimeSpan = 17,
      Guid = 18,
      ByteArray = 19,
      CharArray = 20,
#if NET8_0_OR_GREATER
      DateOnly = 21,
      TimeOnly = 22
#endif
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
