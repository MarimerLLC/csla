#nullable disable
using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a legacy version of <see cref="CslaBinaryReader"/>. You should
  /// not use this type unless you have issues with the <see cref="CslaBinaryReader"/>.
  /// </summary>
  [Obsolete("This type of serialization is unsupported. It will be removed with one of the next major versions of CSLA.NET.")]
  public class CslaLegacyBinaryReader : ICslaReader
  {
    private readonly Dictionary<int, string> keywordsDictionary;

    /// <summary>
    /// Creates new instance of <see cref="CslaLegacyBinaryReader"/>
    /// </summary>
    public CslaLegacyBinaryReader()
    {
      keywordsDictionary = new Dictionary<int, string>();
    }

    /// <summary>
    /// Read a data from a stream, typically MemoryStream, and convert it into 
    /// a list of SerializationInfo objects
    /// </summary>
    /// <param name="serializationStream">Stream to read the data from</param>
    /// <returns>List of SerializationInfo objects</returns>
    /// <exception cref="ArgumentNullException"><paramref name="serializationStream"/> is <see langword="null"/>.</exception>
    public List<SerializationInfo> Read(Stream serializationStream)
    {
      Guard.NotNull(serializationStream);

      var returnValue = new List<SerializationInfo>();
      keywordsDictionary.Clear();
      using var reader = new BinaryReader(serializationStream);
      var totalCount = reader.ReadInt32();
      for (var counter = 0; counter < totalCount; counter++)
      {
        var info = new SerializationInfo(referenceId: reader.ReadInt32(), typeName: (string)ReadObject(reader));

        var childCount = reader.ReadInt32();
        string systemName;
        bool isDirty;
        for (var childCounter = 0; childCounter < childCount; childCounter++)
        {
          systemName = (string)ReadObject(reader);
          isDirty = (bool)ReadObject(reader);
          var referenceId = (int)ReadObject(reader);
          info.AddChild(systemName, referenceId, isDirty);
        }

        var valueCount = reader.ReadInt32();
        for (var valueCounter = 0; valueCounter < valueCount; valueCounter++)
        {
          systemName = (string)ReadObject(reader);
          var enumTypeName = (string)ReadObject(reader);
          isDirty = (bool)ReadObject(reader);
          var value = ReadObject(reader);
          info.AddValue(systemName, value, isDirty, string.IsNullOrEmpty(enumTypeName) ? null : enumTypeName);
        }
        returnValue.Add(info);
      }

      return returnValue;
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Ignored because the type is outdated and removed with the major version 11.
    private object? ReadObject(BinaryReader reader)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
      var knownType = (CslaKnownTypes)reader.ReadByte();
      switch (knownType)
      {
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
          var totalBits = reader.ReadInt32();
          var decimalBits = new int[totalBits];
          for (var counter = 0; counter < totalBits; counter++)
          {
            decimalBits[counter] = reader.ReadInt32();
          }
          return new Decimal(decimalBits);
        case CslaKnownTypes.DateTime:
          return new DateTime(reader.ReadInt64());
        case CslaKnownTypes.String:
          return reader.ReadString();
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
          var returnValue = new List<int>();
          var total = reader.ReadInt32();
          for (var counter = 0; counter < total; counter++)
          {
            returnValue.Add(reader.ReadInt32());
          }
          return returnValue;
        case CslaKnownTypes.Null:
          return null;
        case CslaKnownTypes.StringWithDictionaryKey:
          var systemString = reader.ReadString();
          keywordsDictionary.Add(reader.ReadInt32(), systemString);
          return systemString;
        case CslaKnownTypes.StringDictionaryKey:
          return keywordsDictionary[reader.ReadInt32()];
        default:
          throw new ArgumentOutOfRangeException(Resources.UnandledKNownTypeException);
      }
    }

  }
}
