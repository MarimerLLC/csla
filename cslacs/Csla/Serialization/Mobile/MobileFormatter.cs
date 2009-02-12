using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Runtime.Serialization;
using Csla.Validation;
using Csla.Properties;

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
      XmlWriter writer = XmlWriter.Create(serializationStream);
      Serialize(writer, graph);
      writer.Flush();
    }

    /// <summary>
    /// Serialize an object graph into XML.
    /// </summary>
    /// <param name="textWriter">
    /// TextWriter to which the serialized data
    /// will be written.
    /// </param>
    /// <param name="graph">
    /// Root object of the object graph
    /// to serialize.
    /// </param>
    public void Serialize(TextWriter textWriter, object graph)
    {
      XmlWriter writer = XmlWriter.Create(textWriter);
      Serialize(writer, graph);
      writer.Flush();
    }

    /// <summary>
    /// Serialize an object graph into XML.
    /// </summary>
    /// <param name="writer">
    /// XmlWriter to which the serialized data
    /// will be written.
    /// </param>
    /// <param name="graph">
    /// Root object of the object graph
    /// to serialize.
    /// </param>
    public void Serialize(XmlWriter writer, object graph)
    {
      _serializationReferences.Clear();

      SerializeObject(graph);

      List<SerializationInfo> serialized = _serializationReferences.Values.ToList();

      DataContractSerializer dc = GetDataContractSerializer();

      dc.WriteObject(writer, serialized);
    }

    private DataContractSerializer GetDataContractSerializer()
    {
      return new DataContractSerializer(
        typeof(List<SerializationInfo>),
        new Type[] { typeof(List<int>), typeof(byte[]), typeof(DateTimeOffset) });
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
        info = new SerializationInfo(_serializationReferences.Count + 1);
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
            thisType.Name));

        if (!_serializationReferences.TryGetValue(mobile, out info))
        {
          info = new SerializationInfo(_serializationReferences.Count + 1);
          _serializationReferences.Add(mobile, info);

          info.TypeName = thisType.AssemblyQualifiedName;

          mobile.GetChildren(info, this);
          mobile.GetState(info);
        }
      }
      return info;
    }

    private Dictionary<IMobileObject, SerializationInfo> _serializationReferences =
      new Dictionary<IMobileObject, SerializationInfo>();

    private static bool IsSerializable(Type objectType)
    {
#if SILVERLIGHT
      return objectType.IsSerializable();
#else
      return objectType.IsSerializable;
#endif
    }

    #endregion

    #region Deserialize

    private Dictionary<int, IMobileObject> _deserializationReferences =
      new Dictionary<int, IMobileObject>();

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
      XmlReader reader = XmlReader.Create(serializationStream);
      return Deserialize(reader);
    }

    /// <summary>
    /// Deserialize an object from XML.
    /// </summary>
    /// <param name="textReader">
    /// TextReader containing the serialized XML
    /// data.
    /// </param>
    /// <returns></returns>
    public object Deserialize(TextReader textReader)
    {
      XmlReader reader = XmlReader.Create(textReader);
      return Deserialize(reader);
    }

    /// <summary>
    /// Deserialize an object from XML.
    /// </summary>
    /// <param name="reader">
    /// XmlReader containing the serialized XML
    /// data.
    /// </param>
    /// <returns></returns>
    public object Deserialize(XmlReader reader)
    {
      DataContractSerializer dc = GetDataContractSerializer();

      List<SerializationInfo> deserialized = dc.ReadObject(reader) as List<SerializationInfo>;

      _deserializationReferences = new Dictionary<int, IMobileObject>();
      foreach (SerializationInfo info in deserialized)
      {
        Type type = Csla.Reflection.MethodCaller.GetType(info.TypeName);
        if(type == null)
          throw new SerializationException(string.Format(
            Resources.MobileFormatterUnableToDeserialize,
            info.TypeName));

#if SILVERLIGHT
        IMobileObject mobile = (IMobileObject)Activator.CreateInstance(type);
#else
        IMobileObject mobile = (IMobileObject)Activator.CreateInstance(type, true);
#endif
        _deserializationReferences.Add(info.ReferenceId, mobile);

        mobile.SetState(info);
      }

      foreach (SerializationInfo info in deserialized)
      {
        IMobileObject mobile = _deserializationReferences[info.ReferenceId];
        mobile.SetChildren(info, this);
      }

      foreach(SerializationInfo info in deserialized)
      {
        ISerializationNotification notifiable = _deserializationReferences[info.ReferenceId] as ISerializationNotification;
        if (notifiable != null)
          notifiable.Deserialized();
      }

      return (_deserializationReferences.Count > 0 ? _deserializationReferences[1] : null);
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
      using (var buffer = new System.IO.MemoryStream(data))
      {
        var formatter = new MobileFormatter();
        return formatter.Deserialize(buffer);
      }
    }

    #endregion
  }
}
