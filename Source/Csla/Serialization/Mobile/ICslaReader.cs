using System.Collections.Generic;
using System.IO;

namespace Csla.Serialization.Mobile
{
	/// <summary>
	/// Represents a reader class that can be used
	/// to read the data sent across the wire in byte array format
	/// when communicating between server and client in both directions
	/// </summary>
	public interface ICslaReader
	{
		/// <summary>
		/// Read the data from a stream and return a list of <see cref="SerializationInfo"/> objects
		/// </summary>
		/// <param name="serializationStream">Stream to read the data from, typically MemoryStream</param>
		/// <returns>List of <see cref="SerializationInfo"/> objects</returns>
		List<SerializationInfo> Read(Stream serializationStream);
	}
}
