using Csla.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a class that is responsible for deserializing <see cref="SerializationInfo"/> objects for
  /// receiving the data from client / server.
  /// </summary>
  public class CslaBinaryReader : ICslaReader
  {
    private readonly Dictionary<int, string> keywordsDictionary;

    /// <summary>
    /// Creates new instance of <see cref="CslaBinaryReader"/>
    /// </summary>
    public CslaBinaryReader()
    {
      keywordsDictionary = new Dictionary<int, string>();
    }

    /// <summary>
    /// Read a data from a stream, typically <see cref="MemoryStream"/>, and convert it into 
    /// a list of <see cref="SerializationInfo"/> objects
    /// </summary>
    /// <param name="serializationStream">Stream to read the data from</param>
    /// <returns>List of <see cref="SerializationInfo"/> objects</returns>
    public List<SerializationInfo> Read(Stream serializationStream)
    {
      var returnValue = new List<SerializationInfo>();
      int childCount, valueCount, referenceId;
      string systemName, enumTypeName;
      bool isDirty;
      object value;
      this.keywordsDictionary.Clear();

      using (var reader = new BinaryReader(serializationStream))
      {
        var totalCount = reader.ReadInt32();
        for (var counter = 0; counter < totalCount; counter++)
        {
          var info = new SerializationInfo()
          {
            ReferenceId = reader.ReadInt32(),
            TypeName = ReadString(reader)
          };

          childCount = reader.ReadInt32();
          for (var childCounter = 0; childCounter < childCount; childCounter++)
          {
            systemName = ReadString(reader);
            isDirty = reader.ReadBoolean();
            referenceId = reader.ReadInt32();
            info.AddChild(systemName, referenceId, isDirty);
          }

          valueCount = reader.ReadInt32();
          for (var valueCounter = 0; valueCounter < valueCount; valueCounter++)
          {
            systemName = ReadString(reader);
            enumTypeName = ReadString(reader);
            isDirty = reader.ReadBoolean();
            value = ReadObject(reader);
            info.AddValue(systemName, value, isDirty, string.IsNullOrEmpty(enumTypeName) ? null : enumTypeName);
          }
          returnValue.Add(info);
        }
      }

      return returnValue;
    }

    private string ReadString(BinaryReader reader) =>
      this.ReadString(reader, (CslaKnownTypes)reader.ReadByte());

    private string ReadString(BinaryReader reader, CslaKnownTypes knownType)
    {
      switch (knownType)
      {
        case CslaKnownTypes.String:
          return reader.ReadString();
        case CslaKnownTypes.StringWithDictionaryKey:
          var systemString = reader.ReadString();
          this.keywordsDictionary.Add(reader.ReadInt32(), systemString);
          return systemString;
        case CslaKnownTypes.StringDictionaryKey:
          return this.keywordsDictionary[reader.ReadInt32()];
        default:
          throw new ArgumentOutOfRangeException(Resources.UnandledKNownTypeException);
      }
    }

    private object ReadObject(BinaryReader reader)
    {
      var knownType = (CslaKnownTypes)reader.ReadByte();
      switch (knownType)
      {
        case CslaKnownTypes.IMobileObject:
          using (MemoryStream arrayBuffer = new MemoryStream(reader.ReadBytes(reader.ReadInt32())))
          {
            var formatter = SerializationFormatterFactory.GetFormatter();
            var obj = formatter.Deserialize(arrayBuffer);
            return obj;
          }
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
        case CslaKnownTypes.TimeSpan:
          return new TimeSpan(reader.ReadInt64());
        case CslaKnownTypes.DateTimeOffset:
          return new DateTimeOffset(reader.ReadInt64(), new TimeSpan(reader.ReadInt64()));
        case CslaKnownTypes.Guid:
          return new Guid(reader.ReadBytes(16));  // 16 bytes in a Guid
        case CslaKnownTypes.ByteArray:
          return reader.ReadBytes(reader.ReadInt32());
        case CslaKnownTypes.ByteArrayArray:
          var count = reader.ReadInt32();
          var result = new byte[count][];
          for (int i = 0; i < count; i++)
            result[i] = reader.ReadBytes(reader.ReadInt32());
          return result;
        case CslaKnownTypes.CharArray:
          return reader.ReadChars(reader.ReadInt32());
        case CslaKnownTypes.ListOfInt:
          var total = reader.ReadInt32();
          var buffer = new int[total];
          for (var counter = 0; counter < total; counter++)
          {
            buffer[counter] = reader.ReadInt32();
          }
          return new List<int>(buffer);
        case CslaKnownTypes.Null:
          return null;
        case CslaKnownTypes.String:
        case CslaKnownTypes.StringWithDictionaryKey:
        case CslaKnownTypes.StringDictionaryKey:
          return ReadString(reader, knownType);
        default:
          throw new ArgumentOutOfRangeException(Resources.UnandledKNownTypeException);
      }
    }
  }
}
