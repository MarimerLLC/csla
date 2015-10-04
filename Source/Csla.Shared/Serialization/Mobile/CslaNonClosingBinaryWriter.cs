using System.IO;

namespace Csla.Serialization.Mobile
{
	/// <summary>
	/// This class is used to get around the issue in .NET framework, where
	/// underlying stream is closed by a writer that writes to that stream
	/// when said writer is disposed
	/// </summary>
	public class CslaNonClosingBinaryWriter : BinaryWriter
	{
		/// <summary>
		/// New instance of CslaNonClosingBinaryWriter
		/// </summary>
		/// <param name="stream">Stream that the writer will write to</param>
		public CslaNonClosingBinaryWriter(Stream stream)
			: base(stream)
		{

		}
		/// <summary>
		/// Overwrite the Dispose method of the base class
		/// in order to keep the stream open
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			// do nothing to keep the stream from closing
		}
	}
}
