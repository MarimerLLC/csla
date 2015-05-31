using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Csla.Serialization.Mobile
{
	/// <summary>
	/// This class uses <see cref="DataContractSerializer"/> and <see cref="XmlDictionaryWriter"/> classes
	/// to write the data to a stream
	/// </summary>
	public class CslaXmlBinaryWriter : ICslaWriter
	{
		/// <summary>
		/// Write the data to a stream
		/// </summary>
		/// <param name="serializationStream">Stream to read the data from</param>
		/// <param name="objectData">List of <see cref="SerializationInfo"/> objects to write</param>
		public void Write(Stream serializationStream, List<SerializationInfo> objectData)
		{
			using (var xmlWrtier = XmlDictionaryWriter.CreateBinaryWriter(serializationStream, null, null, false))
			{
				DataContractSerializer dataContractSerializer = CslaReaderWriterFactory.GetDataContractSerializer();
				dataContractSerializer.WriteObject(xmlWrtier, objectData);
				xmlWrtier.Flush();
			}
		}
	}
}
