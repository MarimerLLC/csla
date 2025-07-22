using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This is a class that is responsible for serializing SerializationInfo objects 
  /// into a Stream for sending the data t client / server
  /// </summary>
  public class CslaBinaryWriter : ICslaWriter
  {
    private readonly Dictionary<string, int> _keywordsDictionary;
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Create new instance of CslaBinaryWriter class
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public CslaBinaryWriter(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      _keywordsDictionary = new Dictionary<string, int>();
    }

    /// <inheritdoc />
    public void Write(Stream serializationStream, List<SerializationInfo> objectData)
    {
      if (serializationStream is null)
        throw new ArgumentNullException(nameof(serializationStream));
      if (objectData is null)
        throw new ArgumentNullException(nameof(objectData));

      _keywordsDictionary.Clear();
      using var writer = new CslaNonClosingBinaryWriter(serializationStream);
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

      if (_keywordsDictionary.TryGetValue(value, out var key))
      {
        returnValue = new DictionaryCheckResult(false, key);
      }
      else
      {
        returnValue = new DictionaryCheckResult(true, _keywordsDictionary.Count);
        _keywordsDictionary.Add(value, returnValue.Key);
      }
      return returnValue;
    }

    private void Write(object? target, BinaryWriter writer)
    {
      if (target == null)
      {
        Write(CslaKnownTypes.Null, writer);
      }
      else if (target is IMobileObject)
      {
        using var buffer = new MemoryStream();
        var formatter = _applicationContext.GetRequiredService<ISerializationFormatter>();
        formatter.Serialize(buffer, target);
        var data = buffer.ToArray();
        Write(CslaKnownTypes.IMobileObject, writer);
        writer.Write(data.Length);
        writer.Write(data);
      }
      else if (target is CslaKnownTypes types)
      {
        writer.Write((byte)types);
      }
      else if (target is TimeSpan span)
      {
        Write(CslaKnownTypes.TimeSpan, writer);
        writer.Write(span.Ticks);
      }
      else if (target is DateTimeOffset offset)
      {
        Write(CslaKnownTypes.DateTimeOffset, writer);
        writer.Write(offset.Ticks);
        writer.Write(offset.Offset.Ticks);
      }
      else if (target is byte[] bytes)
      {
        Write(CslaKnownTypes.ByteArray, writer);
        writer.Write(bytes.Length);
        writer.Write(bytes);
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
      else if (target is char[] chars)
      {
        Write(CslaKnownTypes.CharArray, writer);
        writer.Write(chars.Length);
        writer.Write(chars);
      }
      else if (target is Guid guid)
      {
        Write(CslaKnownTypes.Guid, writer);
        writer.Write(guid.ToByteArray());
      }
      else if (target is List<int> ints)
      {
        Write(CslaKnownTypes.ListOfInt, writer);
        writer.Write(ints.Count);
        foreach (var oneInt in ints)
        {
          writer.Write(oneInt);
        }
      }
#if NET8_0_OR_GREATER
      else if (target is DateOnly dateOnly)
      {
        Write(CslaKnownTypes.DateOnly, writer);
        var value = dateOnly.ToDateTime(TimeOnly.MinValue).Ticks;
        writer.Write(value);
      }
      else if (target is TimeOnly timeOnly)
      {
        Write(CslaKnownTypes.TimeOnly, writer);
        var value = timeOnly.Ticks;
        writer.Write(value);
      }
#endif
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
