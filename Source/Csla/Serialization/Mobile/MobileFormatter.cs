﻿//-----------------------------------------------------------------------
// <copyright file="SerializationFormatterFactory.GetFormatter().cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Serializes and deserializes objects</summary>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Serializes and deserializes objects
  /// at the field level. 
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public sealed class MobileFormatter : ISerializationFormatter
  {
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public MobileFormatter(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

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
      ICslaWriter writer = CslaReaderWriterFactory.GetCslaWriter(_applicationContext);
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
    /// <para>
    /// Converts any enum values in the <see cref="SerializationInfo" /> objects to
    /// integer representations. Normally, <see cref="DataContractSerializer" /> requires
    /// all non-standard primitive types to be provided to it's constructor both upon
    /// serialization and deserialization. Since there is no way of knowing what enum
    /// values are being deserialized, there is no way to supply the types to the constructor
    /// at the time of deserialization.
    /// </para>
    /// <para>
    /// Instead we convert the enum values to integers prior to serialization and then back
    /// to proper enum objects after deserialization.
    /// </para>
    /// </summary>
    /// <seealso cref="ConvertEnumsFromIntegers" />
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

    /// <summary>
    /// Serializes an object into a SerializationInfo object.
    /// </summary>
    /// <param name="obj">Object to be serialized.</param>
    public SerializationInfo SerializeObject(object obj)
    {
      SerializationInfo info;
      if (obj == null)
      {
        NullPlaceholder nullPlaceholder = new NullPlaceholder();

        info = new SerializationInfo(_serializationReferences.Count + 1);
        _serializationReferences.Add(nullPlaceholder, info);

        info.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(typeof(NullPlaceholder));
      }
      else
      {
        var thisType = obj.GetType();
        if (obj is System.Security.Claims.ClaimsPrincipal cp)
        {
          obj = new Security.CslaClaimsPrincipal(cp);
          thisType = obj.GetType();
        }
        if (obj is not IMobileObject mobile)
          throw new InvalidOperationException(
            string.Format(Resources.MustImplementIMobileObject,
            thisType.Name));

        if (!_serializationReferences.TryGetValue(mobile, out info))
        {
          info = new SerializationInfo(_serializationReferences.Count + 1);
          _serializationReferences.Add(mobile, info);

          info.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(thisType);
          if (thisType.Equals(typeof(Security.CslaClaimsPrincipal)))
          {
            var principal = (Security.CslaClaimsPrincipal)obj;
            using var buffer = new MemoryStream();
            using var writer = new BinaryWriter(buffer);
            principal.WriteTo(writer);
            info.AddValue("s", buffer.ToArray());
          }
          else
          {
            mobile.GetChildren(info, this);
            mobile.GetState(info);
          }
        }
      }
      return info;
    }

    private Dictionary<IMobileObject, SerializationInfo> _serializationReferences =
      new Dictionary<IMobileObject, SerializationInfo>(new ReferenceComparer<IMobileObject>());

#endregion

#region Deserialize

    private Dictionary<int, IMobileObject> _deserializationReferences = [];

    private Dictionary<string, Type> _typeCache = [];

    private Type GetTypeFromCache(string typeName)
    {
      Type result;
      if (!_typeCache.TryGetValue(typeName, out result))
      {
        result = MethodCaller.GetType(typeName);

        if (result == null)
        {
          throw new SerializationException(string.Format(
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

      ICslaReader reader = CslaReaderWriterFactory.GetCslaReader(_applicationContext);
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
      ICslaReader reader = CslaReaderWriterFactory.GetCslaReader(_applicationContext);
      var data = reader.Read(serializationStream);
      return DeserializeAsDTO(data);
    }

    /// <summary>
    /// Deserialize an object from DTO graph.
    /// </summary>
    ///<param name="deserialized">DTO group to deserialize</param>
    public object DeserializeAsDTO(List<SerializationInfo> deserialized)
    {

      _deserializationReferences = new Dictionary<int, IMobileObject>();
      foreach (SerializationInfo info in deserialized)
      {
        var typeName = AssemblyNameTranslator.GetAssemblyQualifiedName(info.TypeName);
        Type type = GetTypeFromCache(typeName);

        if (type == null)
        {
          throw new SerializationException(string.Format(
            Resources.MobileFormatterUnableToDeserialize, typeName));
        }
        else if (type == typeof(NullPlaceholder))
        {
          _deserializationReferences.Add(info.ReferenceId, null);
        }
        else
        {
          if (type.Equals(typeof(Security.CslaClaimsPrincipal)))
          {
            var state = info.GetValue<byte[]>("s");
            using var buffer = new MemoryStream(state);
            using var reader = new BinaryReader(buffer);
            var mobile = new Security.CslaClaimsPrincipal(reader);
            _deserializationReferences.Add(info.ReferenceId, mobile);
          }
          else
          {
            IMobileObject mobile = (IMobileObject)_applicationContext.CreateInstance(type);

            _deserializationReferences.Add(info.ReferenceId, mobile);

            ConvertEnumsFromIntegers(info);
            mobile.SetState(info);
          }
        }
      }

      foreach (SerializationInfo info in deserialized)
      {
        IMobileObject mobile = _deserializationReferences[info.ReferenceId];

        mobile?.SetChildren(info, this);
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
    public IMobileObject GetObject(int referenceId)
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
      var formatter = new MobileFormatter(_applicationContext);
      formatter.Serialize(buffer, obj);
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
      var formatter = new MobileFormatter(_applicationContext);
      return formatter.SerializeAsDTO(obj);
    }

    /// <summary>
    /// Serializes an object from a DTO graph
    /// </summary>
    /// <param name="serialized">DTO Graph to deserialize</param>
    /// <returns>Deserialized object</returns>
    public object DeserializeFromDTO(List<SerializationInfo> serialized)
    {
      var formatter = new MobileFormatter(_applicationContext);
      return formatter.DeserializeAsDTO(serialized);
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
      var formatter = new MobileFormatter(_applicationContext);
      return formatter.Deserialize(buffer);
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

      var formatter = new MobileFormatter(_applicationContext);
      return formatter.DeserializeAsDTO(data);
    }
#endregion

  }
}