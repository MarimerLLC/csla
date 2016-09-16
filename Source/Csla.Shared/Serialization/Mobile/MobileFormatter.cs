﻿//-----------------------------------------------------------------------
// <copyright file="MobileFormatter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Serializes and deserializes objects</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Serializes and deserializes objects
  /// at the field level. A Silverlight-
  /// compatible facsimile of the
  /// BinaryFormatter or NetDataContractSerializer.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public sealed class MobileFormatter : ISerializationFormatter
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
      ICslaWriter writer = CslaReaderWriterFactory.GetCslaWriter();
      writer.Write(serializationStream, SerializeToDTO(graph));
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

#if NETFX_CORE
            if (fieldType.IsEnum())
#else
            if (fieldType.IsEnum)
#endif
            {
#if (ANDROID || IOS) || NETFX_CORE
              fieldData.Value = Convert.ChangeType(fieldData.Value, Enum.GetUnderlyingType(fieldType), CultureInfo.CurrentCulture);
#else
              fieldData.Value = Convert.ChangeType(fieldData.Value, Enum.GetUnderlyingType(fieldType));
#endif
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
    /// <returns></returns>
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
        if (!IsSerializable(thisType))
          throw new InvalidOperationException(
            string.Format(Resources.ObjectNotSerializableFormatted, thisType.FullName));
        var mobile = obj as IMobileObject;
        if (mobile == null)
          throw new InvalidOperationException(
            string.Format(Resources.MustImplementIMobileObject,
#if NETFX_CORE
            thisType.Name()));
#else
            thisType.Name));
#endif

        if (!_serializationReferences.TryGetValue(mobile, out info))
        {
          info = new SerializationInfo(_serializationReferences.Count + 1);
          _serializationReferences.Add(mobile, info);

          info.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(thisType);

          mobile.GetChildren(info, this);
          mobile.GetState(info);
        }
      }
      return info;
    }

    private Dictionary<IMobileObject, SerializationInfo> _serializationReferences =
      new Dictionary<IMobileObject, SerializationInfo>(new ReferenceComparer<IMobileObject>());

    private static bool IsSerializable(Type objectType)
    {
      return objectType.IsSerializable();
    }

#endregion

#region Deserialize

    private Dictionary<int, IMobileObject> _deserializationReferences =
      new Dictionary<int, IMobileObject>();

    private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

    private Type GetTypeFromCache(string typeName)
    {
      Type result;
      if (!_typeCache.TryGetValue(typeName, out result))
      {
        result = Csla.Reflection.MethodCaller.GetType(typeName);

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
    /// <returns></returns>
    public object Deserialize(Stream serializationStream)
    {
      if (serializationStream == null)
        return null;

      ICslaReader reader = CslaReaderWriterFactory.GetCslaReader();
      var data = reader.Read(serializationStream);
      return DeserializeAsDTO(data);
    }


    /// <summary>
    /// Deserialize an object from DTO graph.
    /// </summary>
    ///<param name="deserialized">DTO grap to deserialize</param>
    /// <returns></returns>
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
#if (ANDROID || IOS) || NETFX_CORE
          IMobileObject mobile = (IMobileObject)Activator.CreateInstance(type);
#else
          IMobileObject mobile = (IMobileObject)Activator.CreateInstance(type, true);
#endif

          _deserializationReferences.Add(info.ReferenceId, mobile);

          ConvertEnumsFromIntegers(info);
          mobile.SetState(info);
        }
      }

      foreach (SerializationInfo info in deserialized)
      {
        IMobileObject mobile = _deserializationReferences[info.ReferenceId];

        if (mobile != null)
        {
          mobile.SetChildren(info, this);
        }
      }

      foreach (SerializationInfo info in deserialized)
      {
        ISerializationNotification notifiable = _deserializationReferences[info.ReferenceId] as ISerializationNotification;
        if (notifiable != null)
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
    /// <returns></returns>
    public IMobileObject GetObject(int referenceId)
    {
      return _deserializationReferences[referenceId];
    }

#endregion

#region Static Helpers

    /// <summary>
    /// Serializes the object into a byte array.
    /// </summary>
    /// <param name="obj">
    /// The object to be serialized, which must implement
    /// IMobileObject.
    /// </param>
    /// <returns></returns>
    public static byte[] Serialize(object obj)
    {
      using (var buffer = new System.IO.MemoryStream())
      {
        var formatter = new MobileFormatter();
        formatter.Serialize(buffer, obj);
        return buffer.ToArray();
      }
    }

    /// <summary>
    /// Serializes the object into a DTO.
    /// </summary>
    /// <param name="obj">
    /// The object to be serialized, which must implement
    /// IMobileObject.
    /// </param>
    /// <returns></returns>
    public static List<SerializationInfo> SerializeToDTO(object obj)
    {
      var formatter = new MobileFormatter();
      return formatter.SerializeAsDTO(obj);
    }

    /// <summary>
    /// Serializes an object from a DTO graph
    /// </summary>
    /// <param name="serialized">DTO Graph to deserialize</param>
    /// <returns>Deserialized object</returns>
    public static object DeserializeFromDTO(List<SerializationInfo> serialized)
    {
      var formatter = new MobileFormatter();
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
    public static object Deserialize(byte[] data)
    {
      if (data == null)
        return null;

      using (var buffer = new System.IO.MemoryStream(data))
      {
        var formatter = new MobileFormatter();
        return formatter.Deserialize(buffer);
      }
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
    public static object Deserialize(List<SerializationInfo> data)
    {
      if (data == null)
        return null;

      var formatter = new MobileFormatter();
      return formatter.DeserializeAsDTO(data);
    }
#endregion

  }
}