//-----------------------------------------------------------------------
// <copyright file="MobileFormatter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Serializes and deserializes objects</summary>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Csla.Configuration;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Serializes and deserializes objects
  /// at the field level. 
  /// </summary>
  /// <remarks>
  /// Creates an instance of the type.
  /// </remarks>
  /// <param name="applicationContext"></param>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public sealed class MobileFormatter(ApplicationContext applicationContext) : ISerializationFormatter
  {
    #region Serialize

    /// <summary>
    /// Serialize an object graph into XML.
    /// </summary>
    /// <param name="serializationStream">
    /// Stream to which the serialized data
    /// will be written.
    /// </param>
    /// <param name="graph">
    /// Root object of the object graph
    /// to serialize.
    /// </param>
    public void Serialize(Stream serializationStream, object graph)
    {
      ICslaWriter writer = CslaReaderWriterFactory.GetCslaWriter(applicationContext);
      writer.Write(serializationStream, SerializeToDTO(graph));
    }

    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="graph">Object graph to be serialized.</param>
    byte[] ISerializationFormatter.Serialize(object graph)
    {
      using var buffer = new MemoryStream();
      Serialize(buffer, graph);
      buffer.Position = 0;
      return buffer.ToArray();
    }

    /// <summary>
    /// Serialize an object graph into DTO.
    /// </summary>
    /// <param name="graph">
    /// Root object of the object graph
    /// to serialize.
    /// </param>
    public List<SerializationInfo> SerializeAsDTO(object graph)
    {
      _serializationReferences.Clear();

      SerializeObject(graph);

      ConvertEnumsToIntegers();
      List<SerializationInfo> serialized = _serializationReferences.Values.ToList();

      return serialized;
    }

    /// <summary>
    /// Converts any enum values in the <see cref="SerializationInfo" /> objects to
    /// integer representations.
    /// </summary>
    private void ConvertEnumsToIntegers()
    {
      foreach (SerializationInfo serializationInfo in _serializationReferences.Values)
      {
        foreach (SerializationInfo.FieldData fieldData in serializationInfo.Values.Values)
        {
          if (fieldData.Value != null)
          {
            Type fieldType = fieldData.Value.GetType();

            if (fieldType.IsEnum)
            {
              fieldData.Value = Convert.ChangeType(fieldData.Value, Enum.GetUnderlyingType(fieldType));
              fieldData.EnumTypeName = fieldType.AssemblyQualifiedName;
            }
          }
        }
      }
    }

    private MobileFormatterOptions GetOptions()
    {
      return applicationContext.GetRequiredService<MobileFormatterOptions>();
    }

    /// <summary>
    /// Serializes an object into a SerializationInfo object.
    /// </summary>
    /// <param name="obj">Object to be serialized.</param>
    public SerializationInfo SerializeObject(object obj)
    {
      var options = GetOptions();
      SerializationInfo info;
      if (obj == null)
      {
        NullPlaceholder nullPlaceholder = new NullPlaceholder();

        info = new SerializationInfo(_serializationReferences.Count + 1);
        _serializationReferences.Add(nullPlaceholder, info);

        info.TypeName = AssemblyNameTranslator.GetSerializationName(typeof(NullPlaceholder), options.UseStrongNamesCheck);
      }
      else if (!_serializationReferences.TryGetValue(obj, out info))
      {
        if (obj is IMobileObject mobile)
        {
          info = new SerializationInfo(_serializationReferences.Count + 1);
          _serializationReferences.Add(mobile, info);

          info.TypeName = AssemblyNameTranslator.GetSerializationName(obj.GetType(), options.UseStrongNamesCheck);
          mobile.GetChildren(info, this);
          mobile.GetState(info);
        }
        else
        {
          var serializerType = options.CustomSerializers.FirstOrDefault(
            s => s.CanSerialize(obj.GetType()))?.SerializerType ??
            throw new InvalidOperationException(string.Format(Resources.MustImplementIMobileObject, obj.GetType().Name));

          var serializer = (IMobileSerializer)applicationContext.CreateInstanceDI(serializerType);
          info = new SerializationInfo(_serializationReferences.Count + 1);
          _serializationReferences.Add(obj, info);
          info.TypeName = AssemblyNameTranslator.GetSerializationName(obj.GetType(), options.UseStrongNamesCheck);
          try
          {
            serializer.Serialize(obj, info);
          }
          catch (Exception ex)
          {
            throw new MobileFormatterException(
              $"CustomSerializerType:{serializerType}; objectType:{obj.GetType()}", ex);
          }
        }
      }

      if (info is null)
      {
        throw new MobileFormatterException(
          string.Format(Resources.MustImplementIMobileObject,
          obj.GetType().Name));
      }
      return info;
    }

    private Dictionary<object, SerializationInfo> _serializationReferences =
      new(new ReferenceComparer<object>());

#endregion

#region Deserialize

    private Dictionary<int, object> _deserializationReferences = [];

    private Dictionary<string, Type> _typeCache = [];

    private Type GetTypeFromCache(string typeName)
    {
      Type result;
      if (!_typeCache.TryGetValue(typeName, out result))
      {
        result = MethodCaller.GetType(typeName);

        if (result == null)
        {
          throw new MobileFormatterException(string.Format(
            Resources.MobileFormatterUnableToDeserialize,
            typeName));
        }
        _typeCache.Add(typeName, result);
      }

      return result;
    }

    /// <summary>
    /// Deserialize an object from XML.
    /// </summary>
    /// <param name="serializationStream">
    /// Stream containing the serialized XML
    /// data.
    /// </param>
    public object Deserialize(Stream serializationStream)
    {
      if (serializationStream == null)
        return null;

      ICslaReader reader = CslaReaderWriterFactory.GetCslaReader(applicationContext);
      var data = reader.Read(serializationStream);
      return DeserializeAsDTO(data);
    }

    /// <summary>
    /// Deserialize an object from XML.
    /// </summary>
    /// <param name="buffer">
    /// Stream containing the serialized XML
    /// data.
    /// </param>
    object ISerializationFormatter.Deserialize(byte[] buffer)
    {
      if (buffer.Length == 0)
        return null;
      using var serializationStream = new MemoryStream(buffer);
      ICslaReader reader = CslaReaderWriterFactory.GetCslaReader(applicationContext);
      var data = reader.Read(serializationStream);
      return DeserializeAsDTO(data);
    }

    /// <summary>
    /// Deserialize an object from DTO graph.
    /// </summary>
    ///<param name="deserialized">DTO group to deserialize</param>
    public object DeserializeAsDTO(List<SerializationInfo> deserialized)
    {

      _deserializationReferences = [];
      foreach (SerializationInfo info in deserialized)
      {
        var typeName = AssemblyNameTranslator.GetAssemblyQualifiedName(info.TypeName);
        Type type = GetTypeFromCache(typeName);

        if (type == null)
        {
          throw new MobileFormatterException(string.Format(
            Resources.MobileFormatterUnableToDeserialize, typeName));
        }
        else if (type == typeof(NullPlaceholder))
        {
          _deserializationReferences.Add(info.ReferenceId, null);
        }
        else if (typeof(IMobileObject).IsAssignableFrom(type))
        {
          var mobile = (IMobileObject)applicationContext.CreateInstance(type);
          _deserializationReferences.Add(info.ReferenceId, mobile);
          ConvertEnumsFromIntegers(info);
          mobile.SetState(info);
        }
        else
        {
          var options = GetOptions();
          var serializerType = options.CustomSerializers.FirstOrDefault(
            s => s.CanSerialize(type))?.SerializerType;
          if (serializerType != null)
          {
            var serializer = (IMobileSerializer)applicationContext.CreateInstanceDI(serializerType);
            object mobile = serializer.Deserialize(info);
            _deserializationReferences.Add(info.ReferenceId, mobile);
          }
          else
          {
            throw new MobileFormatterException(string.Format(
              Resources.MustImplementIMobileObject, type.Name));
          }
        }
      }

      foreach (SerializationInfo info in deserialized)
      {
        var obj = _deserializationReferences[info.ReferenceId];
        if (obj is IMobileObject mobile)
          mobile.SetChildren(info, this);
      }

      foreach (SerializationInfo info in deserialized)
      {
        if (_deserializationReferences[info.ReferenceId] is ISerializationNotification notifiable)
          notifiable.Deserialized();
      }
      return (_deserializationReferences.Count > 0 ? _deserializationReferences[1] : null);
    }

    /// <summary>
    /// Converts any enum values in the <see cref="SerializationInfo" /> object from their
    /// integer representations to normal enum objects.
    /// </summary>
    /// <seealso cref="ConvertEnumsToIntegers" />
    private static void ConvertEnumsFromIntegers(SerializationInfo serializationInfo)
    {
      foreach (SerializationInfo.FieldData fieldData in serializationInfo.Values.Values)
      {
        if (!String.IsNullOrEmpty(fieldData.EnumTypeName))
        {
          Type enumType = MethodCaller.GetType(fieldData.EnumTypeName);
          fieldData.Value = Enum.ToObject(enumType, fieldData.Value);
        }
      }
    }

    /// <summary>
    /// Gets a deserialized object based on the object's
    /// reference id within the serialization stream.
    /// </summary>
    /// <param name="referenceId">Id of object in stream.</param>
    public object GetObject(int referenceId)
    {
      return _deserializationReferences[referenceId];
    }

#endregion

#region Helpers

    /// <summary>
    /// Serializes the object into a byte array.
    /// </summary>
    /// <param name="obj">
    /// The object to be serialized, which must implement
    /// IMobileObject.
    /// </param>
    public byte[] SerializeToByteArray(object obj)
    {
      using var buffer = new MemoryStream();
      Serialize(buffer, obj);
      return buffer.ToArray();
    }

    /// <summary>
    /// Serializes the object into a DTO.
    /// </summary>
    /// <param name="obj">
    /// The object to be serialized, which must implement
    /// IMobileObject.
    /// </param>
    public List<SerializationInfo> SerializeToDTO(object obj)
    {
      return SerializeAsDTO(obj);
    }

    /// <summary>
    /// Serializes an object from a DTO graph
    /// </summary>
    /// <param name="serialized">DTO Graph to deserialize</param>
    /// <returns>Deserialized object</returns>
    public object DeserializeFromDTO(List<SerializationInfo> serialized)
    {
      return DeserializeAsDTO(serialized);
    }

    /// <summary>
    /// Deserializes a byte stream into an object.
    /// </summary>
    /// <param name="data">
    /// Byte array containing the object's serialized
    /// data.
    /// </param>
    /// <returns>
    /// An object containing the data from the
    /// byte stream. The object must implement
    /// IMobileObject to be deserialized.
    /// </returns>
    public object DeserializeFromByteArray(byte[] data)
    {
      if (data == null)
        return null;

      using var buffer = new MemoryStream(data);
      return Deserialize(buffer);
    }

    /// <summary>
    /// Deserializes a byte stream into an object.
    /// </summary>
    /// <param name="data">
    /// DTO containing the object's serialized
    /// data.
    /// </param>
    /// <returns>
    /// An object containing the data from the
    /// byte stream. The object must implement
    /// IMobileObject to be deserialized.
    /// </returns>
    public object DeserializeFromSerializationInfo(List<SerializationInfo> data)
    {
      if (data == null)
        return null;

      return DeserializeAsDTO(data);
    }
    #endregion

    /// <summary>
    /// Determines whether the specified type can be serialized as a child.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns></returns>
    public bool IsTypeSerializable(Type type)
    {
      if (typeof(IMobileObject).IsAssignableFrom(type))
        return true;
      var options = GetOptions();
      return options.CustomSerializers.Any(s => s.CanSerialize(type));
    }
  }
}