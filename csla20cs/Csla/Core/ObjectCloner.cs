using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Csla.Core
{
  internal static class ObjectCloner
  {
    public static object Clone(object obj)
    {
      using (MemoryStream buffer = new MemoryStream())
      {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(buffer, obj);
        buffer.Position = 0;
        object temp = formatter.Deserialize(buffer);
        return temp;
      }
    }
  }
}