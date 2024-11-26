using System.Runtime.Serialization;
using System.Xml;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This class uses <see cref="DataContractSerializer"/> and <see cref="XmlDictionaryReader"/> classes
  /// to read the data from a stream
  /// </summary>
  [Obsolete("This type of serialization is unsupported. It will be removed with one of the next major versions of CSLA.NET.")]
  public class CslaXmlBinaryReader : ICslaReader
  {
    /// <summary>
    /// Read the data from a stream and produce list of <see cref="SerializationInfo"/> objects
    /// </summary>
    /// <param name="serializationStream">Stream to read the data from</param>
    /// <returns>List of SerializationInfo objects</returns>
    public List<SerializationInfo> Read(Stream serializationStream)
    {
      using var xmlReader = XmlDictionaryReader.CreateBinaryReader(serializationStream, XmlDictionaryReaderQuotas.Max);
      var dataContractSerializer = CslaReaderWriterFactory.GetDataContractSerializer();
      return (List<SerializationInfo>)dataContractSerializer.ReadObject(xmlReader);
    }
  }
}
