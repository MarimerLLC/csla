using System;
using System.IO;
using ICSharpCode.SharpZipLib;
namespace ExtendableWcfPortalForDotNet.Compression
{
  public class CompressionUtility
  {
    public static byte[] Compress(byte[] byteData)
    {
      byte[] compressedData = null;
      if (byteData != null)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          ICSharpCode.SharpZipLib.Zip.Compression.Deflater defl =
           new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9, false);
          using (Stream s =
              new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms, defl))
            s.Write(byteData, 0, byteData.Length);
          compressedData = ms.ToArray();
        }
      }
      return compressedData;
    }

    public static byte[] Decompress(byte[] byteInput)
    {
      byte[] bytResult = null;
      if (byteInput != null)
      {
        using (MemoryStream ms = new MemoryStream(byteInput, 0, byteInput.Length))
        {
          string strResult = String.Empty;
          byte[] writeData = new byte[4096];
          Stream s2 =
             new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(ms);
          bytResult = ReadFullStream(s2);
        }
      }
      return bytResult;
    }

    private static byte[] ReadFullStream(Stream stream)
    {
      byte[] buffer = new byte[32768];
      using (MemoryStream ms = new MemoryStream())
      {
        while (true)
        {
          int read = stream.Read(buffer, 0, buffer.Length);
          if (read <= 0)
            return ms.ToArray();
          ms.Write(buffer, 0, read);
        }
      }
    }

  }
}
