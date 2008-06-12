using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Runtime.Serialization;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Serializes and deserializes objects
  /// at the field level. A Silverlight-
  /// compatible facsimile of the
  /// BinaryFormatter or NetDataContractSerializer.
  /// </summary>
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
      
      DataContractSerializer dc = new DataContractSerializer(
        typeof(List<SerializationInfo>), 
        new Type[] { typeof(List<int>)});

      dc.WriteObject(writer, serialized);
    }

    internal SerializationInfo SerializeObject(object obj)
    {
      var thisType = obj.GetType();
      if (!IsSerializable(thisType))
        throw new InvalidOperationException(
          string.Format("Object not serializable ({0})", thisType.FullName));
      var mobile = obj as IMobileObject;
      if (mobile == null)
        throw new InvalidOperationException(
          string.Format("Type {0} must implement IMobileObject",
          thisType.Name));

      SerializationInfo info;
      if (!_serializationReferences.TryGetValue(mobile, out info))
      {
        info = new SerializationInfo(_serializationReferences.Count + 1);
        _serializationReferences.Add(mobile, info);

        info.TypeName = thisType.FullName + ", " + thisType.Assembly.FullName;

        mobile.GetChildren(info, this);
        mobile.GetState(info);
      }
      return info;
    }

    private Dictionary<IMobileObject, SerializationInfo> _serializationReferences =
      new Dictionary<IMobileObject, SerializationInfo>();

    private static bool IsSerializable(Type objectType)
    {
      var a = objectType.GetCustomAttributes(typeof(Csla.Serialization.SerializableAttribute), false);
      return a.Length > 0;
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
      DataContractSerializer dc = new DataContractSerializer(
        typeof(List<SerializationInfo>),
        new Type[] { typeof(List<int>) });

      List<SerializationInfo> deserialized = dc.ReadObject(reader) as List<SerializationInfo>;

      _deserializationReferences = new Dictionary<int, IMobileObject>();
      foreach (SerializationInfo info in deserialized)
      {
        Type type = Type.GetType(info.TypeName);
        IMobileObject mobile = (IMobileObject)Activator.CreateInstance(type);
        _deserializationReferences.Add(info.ReferenceId, mobile);

        mobile.SetState(info);
      }

      foreach (SerializationInfo info in deserialized)
      {
        IMobileObject mobile = _deserializationReferences[info.ReferenceId];
        mobile.SetChildren(info, this);
      }

      return _deserializationReferences[1];
    }

    internal IMobileObject GetObject(int referenceId)
    {
      return _deserializationReferences[referenceId];
    }

    #endregion

    #region Static Helpers

    public static byte[] Serialize(object obj)
    {
      var buffer = new System.IO.MemoryStream();
      var formatter = new MobileFormatter();
      formatter.Serialize(buffer, obj);
      return buffer.ToArray();
    }

    public static object Deserialize(byte[] data)
    {
      var buffer = new System.IO.MemoryStream(data);
      var formatter = new MobileFormatter();
      return formatter.Deserialize(buffer);
    }

    #endregion
  }
}
