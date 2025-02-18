using System.Runtime.Serialization;
using System.Xml;
using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This class uses <see cref="DataContractSerializer"/> and <see cref="XmlReader"/> classes
  /// to read the data from a stream
  /// </summary>
  [Obsolete("This type of serialization is unsupported. It will be removed with one of the next major versions of CSLA.NET.")]
  public class CslaXmlReader : ICslaReader
  {
    /// <summary>
    /// Read the data from a stream and produce list of <see cref="SerializationInfo"/> objects
    /// </summary>
    /// <param name="serializationStream">Stream to read the data from</param>
    /// <returns>List of SerializationInfo objects</returns>
    public List<SerializationInfo> Read(Stream serializationStream)
    {
      using var xmlReader = XmlReader.Create(serializationStream);
      var dataContractSerializer = CslaReaderWriterFactory.GetDataContractSerializer();
      return (List<SerializationInfo>)(dataContractSerializer.ReadObject(xmlReader) ?? throw new SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, nameof(List<SerializationInfo>))));
    }
  }
}
