using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Csla.Serialization.Mobile
{
	/// <summary>
	/// This class uses <see cref="DataContractSerializer"/> and <see cref="XmlReader"/> classes
	/// to read the data from a stream
	/// </summary>
	public class CslaXmlReader : ICslaReader
	{
		/// <summary>
		/// Read the data from a stream and produce list of <see cref="SerializationInfo"/> objects
		/// </summary>
		/// <param name="serializationStream">Stream to read the data from</param>
		/// <returns>List of SerializationInfo objects</returns>
		public List<SerializationInfo> Read(Stream serializationStream)
		{
			List<SerializationInfo> returnValue = null;
			using (var xmlReader = XmlReader.Create(serializationStream))
			{
				DataContractSerializer dataContractSerializer = CslaReaderWriterFactory.GetDataContractSerializer();
				returnValue = (List<SerializationInfo>)dataContractSerializer.ReadObject(xmlReader);
			}
			return returnValue;
		}
	}
}
