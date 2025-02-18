using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a class that is responsible for deserializing <see cref="SerializationInfo"/> objects for
  /// receiving the data from client / server.
  /// </summary>
  public class CslaBinaryReader : ICslaReader
  {
    private readonly Dictionary<int, string> keywordsDictionary;
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Creates new instance of <see cref="CslaBinaryReader"/>
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public CslaBinaryReader(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      keywordsDictionary = new Dictionary<int, string>();
    }

    /// <inheritdoc />
    public List<SerializationInfo> Read(Stream serializationStream)
    {
      if (serializationStream is null)
        throw new ArgumentNullException(nameof(serializationStream));

      var returnValue = new List<SerializationInfo>();
      keywordsDictionary.Clear();

      using var reader = new BinaryReader(serializationStream);
      var totalCount = reader.ReadInt32();
      for (var counter = 0; counter < totalCount; counter++)
      {
        var info = new SerializationInfo(referenceId: reader.ReadInt32(), typeName: ReadString(reader));

        var childCount = reader.ReadInt32();
        string systemName;
        bool isDirty;
        for (var childCounter = 0; childCounter < childCount; childCounter++)
        {
          systemName = ReadString(reader);
          isDirty = reader.ReadBoolean();
          var referenceId = reader.ReadInt32();
          info.AddChild(systemName, referenceId, isDirty);
        }

        var valueCount = reader.ReadInt32();
        for (var valueCounter = 0; valueCounter < valueCount; valueCounter++)
        {
          systemName = ReadString(reader);
          var enumTypeName = ReadString(reader);
          isDirty = reader.ReadBoolean();
          var value = ReadObject(reader);
          info.AddValue(systemName, value, isDirty, string.IsNullOrEmpty(enumTypeName) ? null : enumTypeName);
        }
        returnValue.Add(info);
      }

      return returnValue;
    }

    private string ReadString(BinaryReader reader) =>
      ReadString(reader, (CslaKnownTypes)reader.ReadByte());

    private string ReadString(BinaryReader reader, CslaKnownTypes knownType)
    {
      switch (knownType)
      {
        case CslaKnownTypes.String:
          return reader.ReadString();
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

    private object? ReadObject(BinaryReader reader)
    {
      var knownType = (CslaKnownTypes)reader.ReadByte();
      switch (knownType)
      {
        case CslaKnownTypes.IMobileObject:
          using (MemoryStream arrayBuffer = new MemoryStream(reader.ReadBytes(reader.ReadInt32())))
          {
            var formatter = _applicationContext.GetRequiredService<ISerializationFormatter>();
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
#if NET8_0_OR_GREATER
        case CslaKnownTypes.DateOnly:
          return DateOnly.FromDateTime(new DateTime(reader.ReadInt64()));
        case CslaKnownTypes.TimeOnly:
          return new TimeOnly(reader.ReadInt64());
#endif
        default:
          throw new ArgumentOutOfRangeException(Resources.UnandledKNownTypeException);
      }
    }
  }
}
