using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Csla.Core
{
  internal static class ObjectCloner
  {
    /// <summary>
    /// Clones an object by using the
    /// <see cref="BinaryFormatter" />.
    /// </summary>
    /// <param name="obj">The object to clone.</param>
    /// <remarks>
    /// The object to be cloned must be serializable.
    /// </remarks>
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