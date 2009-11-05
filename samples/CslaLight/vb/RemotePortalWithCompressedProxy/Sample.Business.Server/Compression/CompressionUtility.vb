Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports ICSharpCode.SharpZipLib
Namespace Sample.Business.Compression
  Public Class CompressionUtility
	Public Shared Function Compress(ByVal byteData() As Byte) As Byte()
	  Try

		Dim ms As New MemoryStream()
		Dim defl As New ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9, False)
		Dim s As Stream = New ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms, defl)
		s.Write(byteData, 0, byteData.Length)
		s.Close()
		Dim compressedData() As Byte = CType(ms.ToArray(), Byte())
		Return compressedData
	  Catch
		Throw
	  End Try

	End Function

	Public Shared Function Decompress(ByVal byteInput() As Byte) As Byte()
	  Dim ms As New MemoryStream(byteInput, 0, byteInput.Length)
	  Dim bytResult() As Byte = Nothing
	  Dim strResult As String = String.Empty
	  Dim writeData(4095) As Byte
	  Dim s2 As Stream = New ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(ms)
	  Try
		bytResult = ReadFullStream(s2)
		s2.Close()
		Return bytResult
	  Catch
		Throw
	  End Try
	End Function

	Private Shared Function ReadFullStream(ByVal stream As Stream) As Byte()
	  Dim buffer(32767) As Byte
	  Using ms As New MemoryStream()
		Do
		  Dim read As Integer = stream.Read(buffer, 0, buffer.Length)
		  If read <= 0 Then
			Return ms.ToArray()
		  End If
		  ms.Write(buffer, 0, read)
		Loop
	  End Using
	End Function

  End Class
End Namespace
