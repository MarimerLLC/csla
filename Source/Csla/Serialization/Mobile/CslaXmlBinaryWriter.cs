using System.Runtime.Serialization;
using System.Xml;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This class uses <see cref="DataContractSerializer"/> and <see cref="XmlDictionaryWriter"/> classes
  /// to write the data to a stream
  /// </summary>
  [Obsolete("This type of serialization is unsupported. It will be removed with one of the next major versions of CSLA.NET.")]
  public class CslaXmlBinaryWriter : ICslaWriter
  {
    /// <summary>
    /// Write the data to a stream
    /// </summary>
    /// <param name="serializationStream">Stream to read the data from</param>
    /// <param name="objectData">List of <see cref="SerializationInfo"/> objects to write</param>
    public void Write(Stream serializationStream, List<SerializationInfo> objectData)
    {
      using var xmlWriter = XmlDictionaryWriter.CreateBinaryWriter(serializationStream, null, null, false);
      var dataContractSerializer = CslaReaderWriterFactory.GetDataContractSerializer();
      dataContractSerializer.WriteObject(xmlWriter, objectData);
      xmlWriter.Flush();
    }
  }
}
