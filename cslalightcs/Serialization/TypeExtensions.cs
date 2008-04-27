using System;

namespace Csla.Serialization
{
  public static class TypeExtensions
  {
    public static bool IsSerializable(this Type type)
    {
      var result = type.GetCustomAttributes(typeof(SerializableAttribute), false);
      return (result != null && result.Length > 0);
    }
  }
}
