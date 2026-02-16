using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a class that is responsible for deserializing <see cref="SerializationInfo"/> objects for
  /// receiving the data from client / server.
  /// </summary>
  public class CslaBinaryReader : ICslaReader
  {
    private readonly Dictionary<int, string> _keywordsDictionary;
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Creates new instance of <see cref="CslaBinaryReader"/>
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public CslaBinaryReader(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      _keywordsDictionary = new Dictionary<int, string>();
    }

    /// <inheritdoc />
    public List<SerializationInfo> Read(Stream serializationStream)
    {
      if (serializationStream is null)
        throw new ArgumentNullException(nameof(serializationStream));

      var returnValue = new List<SerializationInfo>();
      _keywordsDictionary.Clear();

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
          _keywordsDictionary.Add(reader.ReadInt32(), systemString);
          return systemString;
        case CslaKnownTypes.StringDictionaryKey:
          return _keywordsDictionary[reader.ReadInt32()];
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
        case CslaKnownTypes.ByteArrayArray:
          var count = reader.ReadInt32();
          var result = new byte[count][];
          for (int i = 0; i < count; i++)
            result[i] = reader.ReadBytes(reader.ReadInt32());
          return result;
        case CslaKnownTypes.ListOfInt:
          var total = reader.ReadInt32();
          var buffer = new int[total];
          for (var counter = 0; counter < total; counter++)
          {
            buffer[counter] = reader.ReadInt32();
          }
          return new List<int>(buffer);
        case CslaKnownTypes.StringWithDictionaryKey:
        case CslaKnownTypes.StringDictionaryKey:
          return ReadString(reader, knownType);
        default:
          // Use shared helper for primitive types
          return BinaryValueHelper.ReadValue(reader, knownType);
      }
    }
  }
}
