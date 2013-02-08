using System;
using Csla.Serialization;
using System.IO;

namespace ExtendableWcfPortalForDotNet
{
  public static class ISerializationFormatterExtensions
  {
    public static byte[] Serialize(this ISerializationFormatter formatter, object objectData)
    {
      if (objectData != null)
      {
        using (MemoryStream stream = new MemoryStream())
        {
          formatter.Serialize(stream, objectData);
          return stream.GetBuffer();
        }
      }
      else
      {
        return null;
      }
    }

    public static object Deserialize(this ISerializationFormatter formatter, byte[] objectData)
    {
      if (objectData != null)
      {
        using (MemoryStream stream = new MemoryStream(objectData))
        {
          return formatter.Deserialize(stream);
        }
      }
      else
      {
        return null;
      }
    }
  }
}
