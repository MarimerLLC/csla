using System;
using System.Collections.Generic;
using System.IO;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a legacy version of <see cref="CslaBinaryWriter"/>. You should
  /// not use this type unless you have issues with the <see cref="CslaBinaryWriter"/>.
  /// </summary>
  public class CslaLegacyBinaryWriter : ICslaWriter
  {
    private readonly Dictionary<string, int> keywordsDictionary;

    /// <summary>
    /// Creates new instance of <see cref="CslaLegacyBinaryWriter"/>
    /// </summary>
    public CslaLegacyBinaryWriter()
    {
      keywordsDictionary = new Dictionary<string, int>();
    }


    /// <summary>
    /// Write a list of SerializationInfo objects into stream,
    /// typically MemoryStream
    /// </summary>
    /// <param name="serializationStream">Stream to write the data into</param>
    /// <param name="objectData">List of SerializationInfo objects to write to stream</param>
    public void Write(Stream serializationStream, List<SerializationInfo> objectData)
    {
      keywordsDictionary.Clear();
      using (var writer = new CslaNonClosingBinaryWriter(serializationStream))
      {
        writer.Write(objectData.Count);
        foreach (var serializationInfo in objectData)
        {
          writer.Write(serializationInfo.ReferenceId);
          WriteSystemString(serializationInfo.TypeName, writer);

          writer.Write(serializationInfo.Children.Count);
          foreach (var childData in serializationInfo.Children)
          {
            WriteSystemString(childData.Key, writer);
            Write(childData.Value.IsDirty, writer);
            Write(childData.Value.ReferenceId, writer);
          }
          writer.Write(serializationInfo.Values.Count);
          foreach (var valueData in serializationInfo.Values)
          {
            WriteSystemString(valueData.Key, writer);
            WriteSystemString(valueData.Value.EnumTypeName ?? string.Empty, writer);
            Write(valueData.Value.IsDirty, writer);
            Write(valueData.Value.Value, writer);
          }
        }
        writer.Flush();
      }
    }

    private void WriteSystemString(string systemString, BinaryWriter writer)
    {
      var checkResult = GetKey(systemString);
      if (checkResult.IsNew)
      {
        Write(CslaKnownTypes.StringWithDictionaryKey, writer);
        writer.Write(systemString);
        writer.Write(checkResult.Key);
      }
      else
      {
        Write(CslaKnownTypes.StringDictionaryKey, writer);
        writer.Write(checkResult.Key);
      }
    }

    private DictionaryCheckResult GetKey(string value)
    {
      DictionaryCheckResult returnValue;
      int key;
      if (keywordsDictionary.TryGetValue(value, out key))
      {
        returnValue = new DictionaryCheckResult(false, key);
      }
      else
      {
        returnValue = new DictionaryCheckResult(true, keywordsDictionary.Count);
        keywordsDictionary.Add(value, returnValue.Key);
      }
      return returnValue;
    }

    private void Write(object target, BinaryWriter writer)
    {
      if (target == null)
      {
        Write(CslaKnownTypes.Null, writer);
      }
      else if (target is CslaKnownTypes)
      {
        writer.Write((byte)((CslaKnownTypes)target));
      }
      else if (target is TimeSpan)
      {
        Write(CslaKnownTypes.TimeSpan, writer);
        writer.Write(((TimeSpan)target).Ticks);
      }
      else if (target is DateTimeOffset)
      {
        Write(CslaKnownTypes.DateTimeOffset, writer);
        writer.Write(((DateTimeOffset)target).Ticks);
        writer.Write(((DateTimeOffset)target).Offset.Ticks);
      }
      else if (target is byte[])
      {
        Write(CslaKnownTypes.ByteArray, writer);
        writer.Write(((byte[])target).Length);
        writer.Write((byte[])target);
      }
      else if (target is byte[][] outerArray)
      {
        Write(CslaKnownTypes.ByteArrayArray, writer);
        writer.Write(outerArray.Length);
        foreach (var item in outerArray)
        {
          writer.Write(item.Length);
          writer.Write(item);
        }
      }
      else if (target is char[])
      {
        Write(CslaKnownTypes.CharArray, writer);
        writer.Write(((char[])target).Length);
        writer.Write((char[])target);
      }
      else if (target is Guid)
      {
        Write(CslaKnownTypes.Guid, writer);
        writer.Write(((Guid)target).ToByteArray());
      }
      else if (target is List<int>)
      {
        Write(CslaKnownTypes.ListOfInt, writer);
        writer.Write(((List<int>)target).Count);
        foreach (var oneInt in ((List<int>)target))
        {
          writer.Write(oneInt);
        }
      }
      else
      {
        var typeCode = Type.GetTypeCode(target.GetType());
        switch (typeCode)
        {
          case TypeCode.Boolean:
            Write(CslaKnownTypes.Boolean, writer);
            writer.Write((Boolean)target);
            break;
          case TypeCode.Char:
            Write(CslaKnownTypes.Char, writer);
            writer.Write((Char)target);
            break;
          case TypeCode.SByte:
            Write(CslaKnownTypes.SByte, writer);
            writer.Write((SByte)target);
            break;
          case TypeCode.Byte:
            Write(CslaKnownTypes.Byte, writer);
            writer.Write((Byte)target);
            break;
          case TypeCode.Int16:
            Write(CslaKnownTypes.Int16, writer);
            writer.Write((Int16)target);
            break;
          case TypeCode.UInt16:
            Write(CslaKnownTypes.UInt16, writer);
            writer.Write((UInt16)target);
            break;
          case TypeCode.Int32:
            Write(CslaKnownTypes.Int32, writer);
            writer.Write((Int32)target);
            break;
          case TypeCode.UInt32:
            Write(CslaKnownTypes.UInt32, writer);
            writer.Write((UInt32)target);
            break;
          case TypeCode.Int64:
            Write(CslaKnownTypes.Int64, writer);
            writer.Write((Int64)target);
            break;
          case TypeCode.UInt64:
            Write(CslaKnownTypes.UInt64, writer);
            writer.Write((UInt64)target);
            break;
          case TypeCode.Single:
            Write(CslaKnownTypes.Single, writer);
            writer.Write((Single)target);
            break;
          case TypeCode.Double:
            Write(CslaKnownTypes.Double, writer);
            writer.Write((Double)target);
            break;
          case TypeCode.Decimal:
            Write(CslaKnownTypes.Decimal, writer);
            var bits = Decimal.GetBits((decimal)target);
            writer.Write(bits.Length);
            foreach (var bit in bits)
            {
              writer.Write(bit);
            }
            break;
          case TypeCode.DateTime:
            Write(CslaKnownTypes.DateTime, writer);
            var value = ((DateTime)target).Ticks;
            writer.Write(value);
            break;
          case TypeCode.String:
            Write(CslaKnownTypes.String, writer);
            writer.Write((String)target);
            break;
          default:
            throw new NotSupportedException(
              $"{Resources.BinaryWriterObjectSerializationException} ({target.GetType().FullName})");
        }
      }
    }
  }
}
