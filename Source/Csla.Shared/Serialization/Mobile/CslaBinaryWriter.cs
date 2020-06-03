using System;
using System.Collections.Generic;
using System.IO;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a class that is responsible for serializing SerializationInfo objects 
  /// into a Stream for sending the data t client / server
  /// </summary>
  public class CslaBinaryWriter : ICslaWriter
  {
    private readonly Dictionary<string, int> keywordsDictionary;

    /// <summary>
    /// Create new instance of CslaBinaryWriter class
    /// </summary>
    public CslaBinaryWriter()
    {
      keywordsDictionary = new Dictionary<string, int>();
    }

    /// <summary>
    /// Write a list of <see cref="SerializationInfo"/> objects into stream,
    /// typically <see cref="MemoryStream"/>.
    /// </summary>
    /// <param name="serializationStream">Stream to write the data into</param>
    /// <param name="objectData">List of <see cref="SerializationInfo"/> objects to write to stream</param>
    public void Write(Stream serializationStream, List<SerializationInfo> objectData)
    {
      this.keywordsDictionary.Clear();
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
            writer.Write(childData.Value.IsDirty);
            writer.Write(childData.Value.ReferenceId);
          }
          writer.Write(serializationInfo.Values.Count);
          foreach (var valueData in serializationInfo.Values)
          {
            WriteSystemString(valueData.Key, writer);
            WriteSystemString(valueData.Value.EnumTypeName ?? string.Empty, writer);
            writer.Write(valueData.Value.IsDirty);
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

      if (keywordsDictionary.TryGetValue(value, out var key))
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
      else if (target is IMobileObject)
      {
        using (MemoryStream buffer = new MemoryStream())
        {
          var formatter = SerializationFormatterFactory.GetFormatter();
          formatter.Serialize(buffer, target);
          var data = buffer.ToArray();
          Write(CslaKnownTypes.IMobileObject, writer);
          writer.Write(data.Length);
          writer.Write(data);
        }
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
            writer.Write((bool)target);
            break;
          case TypeCode.Char:
            Write(CslaKnownTypes.Char, writer);
            writer.Write((char)target);
            break;
          case TypeCode.SByte:
            Write(CslaKnownTypes.SByte, writer);
            writer.Write((sbyte)target);
            break;
          case TypeCode.Byte:
            Write(CslaKnownTypes.Byte, writer);
            writer.Write((byte)target);
            break;
          case TypeCode.Int16:
            Write(CslaKnownTypes.Int16, writer);
            writer.Write((short)target);
            break;
          case TypeCode.UInt16:
            Write(CslaKnownTypes.UInt16, writer);
            writer.Write((ushort)target);
            break;
          case TypeCode.Int32:
            Write(CslaKnownTypes.Int32, writer);
            writer.Write((int)target);
            break;
          case TypeCode.UInt32:
            Write(CslaKnownTypes.UInt32, writer);
            writer.Write((uint)target);
            break;
          case TypeCode.Int64:
            Write(CslaKnownTypes.Int64, writer);
            writer.Write((long)target);
            break;
          case TypeCode.UInt64:
            Write(CslaKnownTypes.UInt64, writer);
            writer.Write((ulong)target);
            break;
          case TypeCode.Single:
            Write(CslaKnownTypes.Single, writer);
            writer.Write((float)target);
            break;
          case TypeCode.Double:
            Write(CslaKnownTypes.Double, writer);
            writer.Write((double)target);
            break;
          case TypeCode.Decimal:
            Write(CslaKnownTypes.Decimal, writer);
            var bits = Decimal.GetBits((decimal)target);
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
            writer.Write((string)target);
            break;
          default:
            throw new NotSupportedException(
              $"{Resources.BinaryWriterObjectSerializationException} ({target.GetType().FullName})");
        }
      }
    }
  }
}
