//-----------------------------------------------------------------------
// <copyright file="BinaryValueHelper.cs" company="Marimer LLC">
// Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for reading and writing primitive values in binary format.</summary>
//-----------------------------------------------------------------------

using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Helper class for reading and writing primitive values in binary format.
  /// Used by <see cref="CslaBinaryReader"/> and <see cref="CslaBinaryWriter"/>.
  /// </summary>
  internal static class BinaryValueHelper
  {
    /// <summary>
    /// Writes a primitive value with its type information to the binary writer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="writer">The binary writer.</param>
    /// <exception cref="NotSupportedException">Thrown when the value type is not supported.</exception>
    public static void WriteValue(object? value, BinaryWriter writer)
    {
      if (value == null)
      {
        writer.Write((byte)CslaKnownTypes.Null);
      }
      else if (value is TimeSpan span)
      {
        writer.Write((byte)CslaKnownTypes.TimeSpan);
        writer.Write(span.Ticks);
      }
      else if (value is DateTimeOffset offset)
      {
        writer.Write((byte)CslaKnownTypes.DateTimeOffset);
        writer.Write(offset.Ticks);
        writer.Write(offset.Offset.Ticks);
      }
      else if (value is byte[] bytes)
      {
        writer.Write((byte)CslaKnownTypes.ByteArray);
        writer.Write(bytes.Length);
        writer.Write(bytes);
      }
      else if (value is char[] chars)
      {
        writer.Write((byte)CslaKnownTypes.CharArray);
        writer.Write(chars.Length);
        writer.Write(chars);
      }
      else if (value is Guid guid)
      {
        writer.Write((byte)CslaKnownTypes.Guid);
        writer.Write(guid.ToByteArray());
      }
#if NET8_0_OR_GREATER
      else if (value is DateOnly dateOnly)
      {
        writer.Write((byte)CslaKnownTypes.DateOnly);
        writer.Write(dateOnly.ToDateTime(TimeOnly.MinValue).Ticks);
      }
      else if (value is TimeOnly timeOnly)
      {
        writer.Write((byte)CslaKnownTypes.TimeOnly);
        writer.Write(timeOnly.Ticks);
      }
#endif
      else
      {
        var typeCode = Type.GetTypeCode(value.GetType());
        switch (typeCode)
        {
          case TypeCode.Boolean:
            writer.Write((byte)CslaKnownTypes.Boolean);
            writer.Write((bool)value);
            break;
          case TypeCode.Char:
            writer.Write((byte)CslaKnownTypes.Char);
            writer.Write((char)value);
            break;
          case TypeCode.SByte:
            writer.Write((byte)CslaKnownTypes.SByte);
            writer.Write((sbyte)value);
            break;
          case TypeCode.Byte:
            writer.Write((byte)CslaKnownTypes.Byte);
            writer.Write((byte)value);
            break;
          case TypeCode.Int16:
            writer.Write((byte)CslaKnownTypes.Int16);
            writer.Write((short)value);
            break;
          case TypeCode.UInt16:
            writer.Write((byte)CslaKnownTypes.UInt16);
            writer.Write((ushort)value);
            break;
          case TypeCode.Int32:
            writer.Write((byte)CslaKnownTypes.Int32);
            writer.Write((int)value);
            break;
          case TypeCode.UInt32:
            writer.Write((byte)CslaKnownTypes.UInt32);
            writer.Write((uint)value);
            break;
          case TypeCode.Int64:
            writer.Write((byte)CslaKnownTypes.Int64);
            writer.Write((long)value);
            break;
          case TypeCode.UInt64:
            writer.Write((byte)CslaKnownTypes.UInt64);
            writer.Write((ulong)value);
            break;
          case TypeCode.Single:
            writer.Write((byte)CslaKnownTypes.Single);
            writer.Write((float)value);
            break;
          case TypeCode.Double:
            writer.Write((byte)CslaKnownTypes.Double);
            writer.Write((double)value);
            break;
          case TypeCode.Decimal:
            writer.Write((byte)CslaKnownTypes.Decimal);
            var bits = Decimal.GetBits((decimal)value);
            foreach (var bit in bits)
            {
              writer.Write(bit);
            }
            break;
          case TypeCode.DateTime:
            writer.Write((byte)CslaKnownTypes.DateTime);
            writer.Write(((DateTime)value).Ticks);
            break;
          case TypeCode.String:
            writer.Write((byte)CslaKnownTypes.String);
            writer.Write((string)value);
            break;
          default:
            throw new NotSupportedException(
              $"{Resources.BinaryWriterObjectSerializationException} ({value.GetType().FullName})");
        }
      }
    }

    /// <summary>
    /// Reads a primitive value from the binary reader based on the known type.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The deserialized value, or null.</returns>
    /// <exception cref="NotSupportedException">Thrown when the known type is not supported.</exception>
    public static object? ReadValue(BinaryReader reader)
    {
      var knownType = (CslaKnownTypes)reader.ReadByte();
      return ReadValue(reader, knownType);
    }

    /// <summary>
    /// Reads a primitive value from the binary reader based on the known type.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <param name="knownType">The known type to read.</param>
    /// <returns>The deserialized value, or null.</returns>
    /// <exception cref="NotSupportedException">Thrown when the known type is not supported.</exception>
    public static object? ReadValue(BinaryReader reader, CslaKnownTypes knownType)
    {
      switch (knownType)
      {
        case CslaKnownTypes.Null:
          return null;
        case CslaKnownTypes.Boolean:
          return reader.ReadBoolean();
        case CslaKnownTypes.Char:
          return reader.ReadChar();
        case CslaKnownTypes.SByte:
          return reader.ReadSByte();
        case CslaKnownTypes.Byte:
          return reader.ReadByte();
        case CslaKnownTypes.Int16:
          return reader.ReadInt16();
        case CslaKnownTypes.UInt16:
          return reader.ReadUInt16();
        case CslaKnownTypes.Int32:
          return reader.ReadInt32();
        case CslaKnownTypes.UInt32:
          return reader.ReadUInt32();
        case CslaKnownTypes.Int64:
          return reader.ReadInt64();
        case CslaKnownTypes.UInt64:
          return reader.ReadUInt64();
        case CslaKnownTypes.Single:
          return reader.ReadSingle();
        case CslaKnownTypes.Double:
          return reader.ReadDouble();
        case CslaKnownTypes.Decimal:
          var decimalBits = new int[4];
          for (var counter = 0; counter < 4; counter++)
          {
            decimalBits[counter] = reader.ReadInt32();
          }
          return new decimal(decimalBits);
        case CslaKnownTypes.DateTime:
          return new DateTime(reader.ReadInt64());
        case CslaKnownTypes.String:
          return reader.ReadString();
        case CslaKnownTypes.TimeSpan:
          return new TimeSpan(reader.ReadInt64());
        case CslaKnownTypes.DateTimeOffset:
          return new DateTimeOffset(reader.ReadInt64(), new TimeSpan(reader.ReadInt64()));
        case CslaKnownTypes.Guid:
          return new Guid(reader.ReadBytes(16));
        case CslaKnownTypes.ByteArray:
          return reader.ReadBytes(reader.ReadInt32());
        case CslaKnownTypes.CharArray:
          return reader.ReadChars(reader.ReadInt32());
#if NET8_0_OR_GREATER
        case CslaKnownTypes.DateOnly:
          return DateOnly.FromDateTime(new DateTime(reader.ReadInt64()));
        case CslaKnownTypes.TimeOnly:
          return new TimeOnly(reader.ReadInt64());
#endif
        default:
          throw new NotSupportedException(
            $"{Resources.UnandledKNownTypeException} ({knownType})");
      }
    }
  }
}
