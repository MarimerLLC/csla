
namespace Csla.Serialization.Mobile
{
	/// <summary>
	/// This enumeration contains the list of known types
	/// that <see cref="CslaBinaryReader"/> and <see cref="CslaBinaryWriter"/>know about
	/// </summary>
	public enum CslaKnownTypes : byte
	{
		/// <summary>
		/// Boolean
		/// </summary>
		Boolean = 1,
		/// <summary>
		/// Character/char
		/// </summary>
		Char = 2,
		/// <summary>
		/// Signed byte
		/// </summary>
		SByte = 3,
		/// <summary>
		/// Byte
		/// </summary>
		Byte = 4,
		/// <summary>
		/// Short /Int 16
		/// </summary>
		Int16 = 5,
		/// <summary>
		/// Unsigned short / Int 16
		/// </summary>
		UInt16 = 6,
		/// <summary>
		/// Integer / Int32
		/// </summary>
		Int32 = 7,
		/// <summary>
		/// Unsigned Integer / Int32
		/// </summary>
		UInt32 = 8,
		/// <summary>
		/// Long / Int64
		/// </summary>
		Int64 = 9,
		/// <summary>
		/// Unsigned Long / Int64
		/// </summary>
		UInt64 = 10,
		/// <summary>
		/// Single / single precision floating point
		/// </summary>
		Single = 11,
		/// <summary>
		/// Double / double precision floating point
		/// </summary>
		Double = 12,
		/// <summary>
		/// Decimal
		/// </summary>
		Decimal = 13,
		/// <summary>
		/// Date / time 
		/// </summary>
		DateTime = 14,
		/// <summary>
		/// String
		/// </summary>
		String = 15,
		/// <summary>
		/// TimeSpan - time span
		/// </summary>
		TimeSpan = 16,
		/// <summary>
		/// Date/time plus time zone / DateTimeOffset
		/// </summary>
		DateTimeOffset = 17,
		/// <summary>
		/// Globally unique identifier / Guid
		/// </summary>
		Guid = 18,
		/// <summary>
		/// Array of bytes. can be used to represent images data
		/// </summary>
		ByteArray = 19,
		/// <summary>
		/// Array of characters, not the same as string
		/// </summary>
		CharArray = 20,
		/// <summary>
		/// List of integer / List(of Int)
		/// Used internally for serialization of list based objects
		/// such as BusinessListBase
		/// </summary>
		ListOfInt = 21,
		/// <summary>
		/// Represents null value
		/// </summary>
		Null = 22,
		/// <summary>
		/// Represents string that is supported by internal dictionary
		/// that is used to replace strings with integers to save space
		/// in the serialization process.  This entry contains both string and 
		/// integer value of that string.  This data is used to re-build the 
		/// dictionary on the receiving end
		/// </summary>
		StringWithDictionaryKey = 23,
		/// <summary>
		/// Key that corresponds to internally used string.  On the receiving end this
		/// value will be replaces with the actual string
		/// </summary>
		StringDictionaryKey = 24,
    /// <summary>
    /// Array of array of bytes
    /// </summary>
    ByteArrayArray = 25,
    /// <summary>
    /// IMobileObject serialized into a byte array
    /// </summary>
    IMobileObject
  }
}
