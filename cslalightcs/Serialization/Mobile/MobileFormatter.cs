using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

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

      var document = new XDocument();
      SerializeObject(graph);
      var root = new XElement("g");
      foreach (var item in _serializationReferences)
        root.Add(item.Value.ToXElement());
      document.Add(root);
      document.Save(writer);
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
        mobile.Serialize(info, this);
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
      XDocument doc = XDocument.Load(reader);
      XElement root = (XElement)doc.FirstNode;

      _deserializationReferences.Clear();

      var objects = from e in root.Elements()
                    where e.Name == "o"
                    select e;

      var infos = new Dictionary<int, SerializationInfo>();

      // create basic SerializationInfo objects
      // and empty target objects
      foreach (var item in objects)
      {
        var info = new SerializationInfo(item);
        infos.Add(info.ReferenceId, info);
        var objType = Type.GetType(info.TypeName);
        var mobile = Activator.CreateInstance(objType) as IMobileObject;
        _deserializationReferences.Add(info.ReferenceId, mobile);
      }

      // fill out rest of SerializationInfo
      foreach (var item in objects)
      {
        var referenceId = Convert.ToInt32(item.Attribute("i").Value);
        var info = infos[referenceId];
        info.Deserialize(item, this);
      }

      // deserialize objects
      foreach (var info in infos)
        GetObject(info.Value.ReferenceId).Deserialize(info.Value, this);

      foreach (var obj in _deserializationReferences)
      {
        var notify = obj.Value as ISerializationNotification;
        if (notify != null)
          notify.Deserialized();
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
